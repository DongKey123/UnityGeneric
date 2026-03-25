using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using ExcelDataReader;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework.Core.DataParser.Editor
{
    /// <summary>
    /// Excel 파일을 읽어 JSON으로 변환하는 컨버터입니다.
    ///
    /// [Excel 형식 규칙]
    /// Row 1 : 컬럼 이름  (id, name, attack, ...)
    /// Row 2 : 데이터 타입 (int, float, bool, string, int[], float[], string[])
    /// Row 3+: 데이터
    ///   - # 로 시작하는 행 → 주석 (무시)
    ///   - 빈 행 → 무시
    ///   - 배열 구분자: | (예: 10|20|30)
    ///
    /// # 로 시작하는 시트 이름도 무시됩니다.
    /// 각 시트는 독립된 JSON 파일로 출력됩니다.
    /// </summary>
    public static class ExcelToJsonConverter
    {
        #region Constants

        private const string CommentPrefix = "#";
        private const char ArrayDelimiter = '|';

        #endregion

        #region Public Methods

        /// <summary>
        /// Excel 파일을 읽어 시트별로 JSON 파일을 생성합니다.
        /// </summary>
        /// <param name="excelPath">Excel 파일 경로 (.xlsx)</param>
        /// <param name="outputDir">JSON 출력 폴더 경로</param>
        /// <param name="prettyPrint">JSON 들여쓰기 여부</param>
        /// <returns>변환된 시트 수</returns>
        public static int Convert(string excelPath, string outputDir, bool prettyPrint = true)
        {
            int convertedCount = 0;

            using var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var reader = ExcelReaderFactory.CreateReader(stream);

            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = _ => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = false
                }
            });

            Directory.CreateDirectory(outputDir);

            foreach (DataTable sheet in dataSet.Tables)
            {
                // # 으로 시작하는 시트는 무시
                if (sheet.TableName.StartsWith(CommentPrefix))
                {
                    Debug.Log($"[ExcelToJson] Skipped sheet: {sheet.TableName}");
                    continue;
                }

                var rows = ParseSheet(sheet);
                if (rows == null)
                {
                    Debug.LogWarning($"[ExcelToJson] Sheet '{sheet.TableName}': 유효한 데이터가 없어 건너뜁니다.");
                    continue;
                }

                var formatting = prettyPrint ? Formatting.Indented : Formatting.None;
                var json = JsonConvert.SerializeObject(rows, formatting);
                var outputPath = Path.Combine(outputDir, $"{sheet.TableName}.json");

                File.WriteAllText(outputPath, json, Encoding.UTF8);
                convertedCount++;

                Debug.Log($"[ExcelToJson] Exported: {sheet.TableName}.json ({rows.Count} rows)");
            }

            return convertedCount;
        }

        #endregion

        #region Private Methods

        private static List<Dictionary<string, object>> ParseSheet(DataTable sheet)
        {
            // 헤더 + 타입 행이 최소 2행 필요
            if (sheet.Rows.Count < 2)
                return null;

            var headers = ParseHeaders(sheet.Rows[0], sheet.Columns.Count);
            if (headers.Count == 0)
                return null;

            var types = ParseTypes(sheet.Rows[1], headers.Count);
            var result = new List<Dictionary<string, object>>();

            for (int rowIndex = 2; rowIndex < sheet.Rows.Count; rowIndex++)
            {
                var row = sheet.Rows[rowIndex];

                if (IsCommentRow(row)) continue;
                if (IsEmptyRow(row, headers.Count)) continue;

                var rowData = new Dictionary<string, object>();

                for (int col = 0; col < headers.Count; col++)
                {
                    var raw = row[col]?.ToString().Trim() ?? string.Empty;
                    rowData[headers[col]] = ParseValue(raw, types[col]);
                }

                result.Add(rowData);
            }

            return result;
        }

        private static List<string> ParseHeaders(DataRow row, int columnCount)
        {
            var headers = new List<string>();

            for (int col = 0; col < columnCount; col++)
            {
                var header = row[col]?.ToString().Trim();

                // 빈 헤더가 나오면 그 이후 컬럼은 무시
                if (string.IsNullOrEmpty(header))
                    break;

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

        private static bool IsCommentRow(DataRow row)
        {
            var firstCell = row[0]?.ToString().Trim() ?? string.Empty;
            return firstCell.StartsWith(CommentPrefix);
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

        private static object ParseValue(string value, string type)
        {
            if (string.IsNullOrEmpty(value))
            {
                return type switch
                {
                    "int"      => (object)0,
                    "float"    => 0f,
                    "bool"     => false,
                    "int[]"    => Array.Empty<int>(),
                    "float[]"  => Array.Empty<float>(),
                    "string[]" => Array.Empty<string>(),
                    _          => string.Empty
                };
            }

            return type switch
            {
                "int"      => int.TryParse(value, out var i) ? i : 0,
                "float"    => float.TryParse(value, System.Globalization.NumberStyles.Float,
                                  System.Globalization.CultureInfo.InvariantCulture, out var f) ? f : 0f,
                "bool"     => value.ToLower() is "true" or "1" or "yes",
                "int[]"    => ParseIntArray(value),
                "float[]"  => ParseFloatArray(value),
                "string[]" => value.Split(ArrayDelimiter),
                _          => value
            };
        }

        private static int[] ParseIntArray(string value)
        {
            var parts = value.Split(ArrayDelimiter);
            var result = new int[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                result[i] = int.TryParse(parts[i].Trim(), out var v) ? v : 0;
            return result;
        }

        private static float[] ParseFloatArray(string value)
        {
            var parts = value.Split(ArrayDelimiter);
            var result = new float[parts.Length];
            for (int i = 0; i < parts.Length; i++)
                result[i] = float.TryParse(parts[i].Trim(), System.Globalization.NumberStyles.Float,
                                 System.Globalization.CultureInfo.InvariantCulture, out var v) ? v : 0f;
            return result;
        }

        #endregion
    }
}
