using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using UnityEngine;

namespace Framework.Core.DataParser.Editor
{
    /// <summary>
    /// 시트 하나의 클래스 생성 결과를 담습니다.
    /// </summary>
    public class ClassGenerateResult
    {
        public string SheetName;
        public bool   Success;
        public string Error;
        public string Code;
    }

    /// <summary>
    /// Excel 시트 구조(Row 1 헤더, Row 2 타입)를 읽어 C# 데이터 클래스와 enum을 자동 생성합니다.
    ///
    /// [생성 규칙]
    /// - 시트당 {SheetName}Data.cs 파일 하나 생성
    /// - enum 컬럼은 실제 데이터 행에서 고유 값을 수집하여 enum 타입으로 생성
    /// - enum 이름: {SheetName}{ColName} (예: MonsterType)
    /// - 클래스 필드: camelCase, enum 필드: 해당 enum 타입
    /// - 클래스에 [Serializable] 어트리뷰트 적용
    /// - 파일 상단: // Auto-generated 주석
    /// </summary>
    public static class ExcelClassGenerator
    {
        #region Constants

        private const string CommentPrefix = "#";

        #endregion

        #region Public Methods

        /// <summary>
        /// Excel 파일의 모든 시트를 읽어 C# 클래스 파일을 생성합니다.
        /// </summary>
        /// <param name="excelPath">Excel 파일 경로 (.xlsx)</param>
        /// <param name="outputDir">C# 파일 출력 폴더 경로</param>
        /// <param name="namespaceName">생성될 클래스의 네임스페이스. 비어있으면 네임스페이스 없이 생성</param>
        /// <param name="generateManager">InGameDataManager partial 클래스 생성 여부</param>
        /// <param name="jsonResourcePrefix">Resources 기준 JSON 경로 접두어 (예: "Data")</param>
        /// <returns>시트별 생성 결과 목록</returns>
        public static List<ClassGenerateResult> GenerateFromFile(string excelPath, string outputDir, string namespaceName = "GameData",
            bool generateManager = false, string jsonResourcePrefix = "Data")
        {
            var results = new List<ClassGenerateResult>();

            using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration { UseHeaderRow = false }
            });

            Directory.CreateDirectory(outputDir);

            foreach (DataTable sheet in dataSet.Tables)
            {
                if (sheet.TableName.StartsWith(CommentPrefix))
                {
                    Debug.Log($"[ExcelClassGenerator] Skipped sheet: {sheet.TableName}");
                    continue;
                }

                var result = new ClassGenerateResult { SheetName = sheet.TableName };

                try
                {
                    var code = GenerateClass(sheet, namespaceName, out string error);

                    if (code == null)
                    {
                        result.Success = false;
                        result.Error   = error;
                        Debug.LogWarning($"[ExcelClassGenerator] '{sheet.TableName}' 건너뜀 — {error}");
                    }
                    else
                    {
                        var outputPath = Path.Combine(outputDir, $"{sheet.TableName}Data.cs");
                        File.WriteAllText(outputPath, code, Encoding.UTF8);
                        result.Success = true;
                        result.Code    = code;
                        Debug.Log($"[ExcelClassGenerator] Generated: {sheet.TableName}Data.cs");

                        if (generateManager)
                        {
                            var managerCode = GenerateManagerPartial(sheet, namespaceName, jsonResourcePrefix);
                            if (managerCode != null)
                            {
                                var managerPath = Path.Combine(outputDir, $"InGameDataManager.{sheet.TableName}.Generated.cs");
                                File.WriteAllText(managerPath, managerCode, Encoding.UTF8);
                                Debug.Log($"[ExcelClassGenerator] Generated: InGameDataManager.{sheet.TableName}.Generated.cs");
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Error   = e.Message;
                    Debug.LogError($"[ExcelClassGenerator] '{sheet.TableName}' 생성 실패 — {e.Message}");
                }

                results.Add(result);
            }

            return results;
        }

        /// <summary>
        /// 폴더 내 모든 .xlsx 파일을 읽어 C# 클래스 파일을 생성합니다.
        /// </summary>
        /// <param name="folderPath">Excel 파일이 담긴 폴더 경로</param>
        /// <param name="outputDir">C# 파일 출력 폴더 경로</param>
        /// <param name="namespaceName">생성될 클래스의 네임스페이스</param>
        /// <param name="generateManager">InGameDataManager partial 클래스 생성 여부</param>
        /// <param name="jsonResourcePrefix">Resources 기준 JSON 경로 접두어 (예: "Data")</param>
        /// <returns>시트별 생성 결과 목록</returns>
        public static List<ClassGenerateResult> GenerateFromFolder(string folderPath, string outputDir, string namespaceName = "GameData",
            bool generateManager = false, string jsonResourcePrefix = "Data")
        {
            var files = Directory.GetFiles(folderPath, "*.xlsx", SearchOption.TopDirectoryOnly);

            if (files.Length == 0)
            {
                Debug.LogWarning($"[ExcelClassGenerator] 폴더에 .xlsx 파일이 없습니다: {folderPath}");
                return new List<ClassGenerateResult>();
            }

            var allResults = new List<ClassGenerateResult>();
            foreach (var file in files)
                allResults.AddRange(GenerateFromFile(file, outputDir, namespaceName, generateManager, jsonResourcePrefix));

            return allResults;
        }

        #endregion

        #region Private Methods

        private static string GenerateManagerPartial(DataTable sheet, string dataNamespace, string jsonResourcePrefix)
        {
            if (sheet.Rows.Count < 2) return null;

            var headers = ParseHeaders(sheet.Rows[0], sheet.Columns.Count);
            if (headers.Count == 0) return null;

            var types = ParseTypes(sheet.Rows[1], headers.Count);

            // id 컬럼 탐색
            int idIndex = -1;
            for (int i = 0; i < headers.Count; i++)
            {
                if (headers[i].ToLower() == "id")
                {
                    idIndex = i;
                    break;
                }
            }

            if (idIndex < 0)
            {
                Debug.LogWarning($"[ExcelClassGenerator] '{sheet.TableName}' — id 컬럼 없음, InGameDataManager 연동 생성 건너뜀");
                return null;
            }

            var sheetName    = sheet.TableName;
            var keyType      = GetCSharpType(types[idIndex], sheetName, headers[idIndex]);
            var resourcePath = $"{jsonResourcePrefix}/{sheetName}";
            var methodName   = $"Get{sheetName}";
            var hasNs        = !string.IsNullOrEmpty(dataNamespace);
            var dataUsing    = hasNs ? $"using {dataNamespace};" : string.Empty;

            var sb = new StringBuilder();
            sb.AppendLine("// Auto-generated by ExcelToJson. Do not edit manually.");
            sb.AppendLine("using System.Collections.Generic;");
            if (!string.IsNullOrEmpty(dataUsing))
                sb.AppendLine(dataUsing);
            sb.AppendLine();
            sb.AppendLine("namespace Framework.Core.DataManager");
            sb.AppendLine("{");
            sb.AppendLine("    public partial class InGameDataManager");
            sb.AppendLine("    {");
            sb.AppendLine($"        public Dictionary<{keyType}, {sheetName}Data> {methodName}()");
            sb.AppendLine($"            => LoadAsDictionary<{keyType}, {sheetName}Data>(\"{resourcePath}\", x => x.id);");
            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private static string GenerateClass(DataTable sheet, string namespaceName, out string error)
        {
            error = null;

            if (sheet.Rows.Count < 2)
            {
                error = $"데이터 없음 ({sheet.Rows.Count}행 — 헤더+타입 최소 2행 필요)";
                return null;
            }

            var headers = ParseHeaders(sheet.Rows[0], sheet.Columns.Count);
            if (headers.Count == 0)
            {
                error = "유효한 컬럼 헤더 없음 (Row 1이 비어있음)";
                return null;
            }

            var types = ParseTypes(sheet.Rows[1], headers.Count);

            // enum 컬럼의 고유 값 수집
            var enumColumns = new Dictionary<int, HashSet<string>>();
            for (int col = 0; col < types.Count; col++)
            {
                if (types[col] == "enum")
                    enumColumns[col] = new HashSet<string>();
            }

            for (int rowIndex = 2; rowIndex < sheet.Rows.Count; rowIndex++)
            {
                var row       = sheet.Rows[rowIndex];
                var firstCell = row[0]?.ToString().Trim() ?? string.Empty;

                if (firstCell.StartsWith(CommentPrefix)) continue;
                if (IsEmptyRow(row, headers.Count))      continue;

                foreach (var kvp in enumColumns)
                {
                    var raw = row[kvp.Key]?.ToString().Trim() ?? string.Empty;
                    if (!string.IsNullOrEmpty(raw))
                        kvp.Value.Add(raw);
                }
            }

            var sb        = new StringBuilder();
            var sheetName = sheet.TableName;
            bool hasNs    = !string.IsNullOrEmpty(namespaceName);
            string ind    = hasNs ? "    " : string.Empty;

            sb.AppendLine("// Auto-generated by ExcelToJson. Do not edit manually.");
            sb.AppendLine("using System;");
            sb.AppendLine();

            if (hasNs)
            {
                sb.AppendLine($"namespace {namespaceName}");
                sb.AppendLine("{");
            }

            // enum 타입 생성
            foreach (var kvp in enumColumns)
            {
                var colName  = ToPascalCase(headers[kvp.Key]);
                var enumName = $"{sheetName}{colName}";

                sb.AppendLine($"{ind}public enum {enumName}");
                sb.AppendLine($"{ind}{{");
                sb.AppendLine($"{ind}    Unknown = 0,");

                var usedMembers = new HashSet<string>();
                foreach (var val in kvp.Value)
                {
                    var member = ToEnumMember(val);
                    if (usedMembers.Add(member))
                        sb.AppendLine($"{ind}    {member},");
                }

                sb.AppendLine($"{ind}}}");
                sb.AppendLine();
            }

            // 데이터 클래스 생성
            sb.AppendLine($"{ind}[Serializable]");
            sb.AppendLine($"{ind}public class {sheetName}Data");
            sb.AppendLine($"{ind}{{");

            for (int col = 0; col < headers.Count; col++)
            {
                var fieldName = ToCamelCase(headers[col]);
                var typeName  = GetCSharpType(types[col], sheetName, headers[col]);
                sb.AppendLine($"{ind}    public {typeName} {fieldName};");
            }

            sb.AppendLine($"{ind}}}");

            if (hasNs)
                sb.AppendLine("}");

            return sb.ToString();
        }

        private static List<string> ParseHeaders(DataRow row, int columnCount)
        {
            var headers = new List<string>();
            for (int col = 0; col < columnCount; col++)
            {
                var header = row[col]?.ToString().Trim();
                if (string.IsNullOrEmpty(header)) break;
                headers.Add(header);
            }
            return headers;
        }

        private static List<string> ParseTypes(DataRow row, int count)
        {
            var types = new List<string>();
            for (int col = 0; col < count; col++)
            {
                var type = row[col]?.ToString().Trim().ToLower();
                types.Add(string.IsNullOrEmpty(type) ? "string" : type);
            }
            return types;
        }

        private static bool IsEmptyRow(DataRow row, int columnCount)
        {
            for (int col = 0; col < columnCount; col++)
            {
                if (!string.IsNullOrEmpty(row[col]?.ToString()))
                    return false;
            }
            return true;
        }

        private static string GetCSharpType(string excelType, string sheetName, string colName)
        {
            return excelType switch
            {
                "int"      => "int",
                "float"    => "float",
                "bool"     => "bool",
                "int[]"    => "int[]",
                "float[]"  => "float[]",
                "string[]" => "string[]",
                "enum"     => $"{sheetName}{ToPascalCase(colName)}",
                _          => "string"
            };
        }

        private static string ToPascalCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return char.ToUpper(name[0]) + name.Substring(1);
        }

        private static string ToCamelCase(string name)
        {
            if (string.IsNullOrEmpty(name)) return name;
            return char.ToLower(name[0]) + name.Substring(1);
        }

        private static string ToEnumMember(string value)
        {
            var sb = new StringBuilder();
            if (value.Length == 0 || (!char.IsLetter(value[0]) && value[0] != '_'))
                sb.Append('_');

            foreach (char c in value)
                sb.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');

            return sb.ToString();
        }

        #endregion
    }
}
