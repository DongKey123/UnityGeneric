using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Core.DataParser.Editor
{
    /// <summary>
    /// Assets 폴더 내 .xlsx 파일이 변경될 때 자동으로 JSON 변환을 수행합니다.
    /// C# 클래스 생성이 활성화된 경우 함께 실행됩니다.
    /// 자동 변환 ON/OFF는 ExcelToJsonWindow의 출력 설정에서 제어합니다.
    /// </summary>
    public class ExcelAutoConverter : AssetPostprocessor
    {
        #region Constants

        internal const string AutoConvertKey = "ExcelToJson_AutoConvert";

        #endregion

        #region Properties

        /// <summary>
        /// 자동 변환 활성화 여부. EditorPrefs에 저장됩니다.
        /// </summary>
        public static bool IsEnabled
        {
            get => EditorPrefs.GetBool(AutoConvertKey, false);
            set => EditorPrefs.SetBool(AutoConvertKey, value);
        }

        #endregion

        #region AssetPostprocessor

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!IsEnabled)
                return;

            var outputPath      = EditorPrefs.GetString(ExcelToJsonWindow.OutputPathKey, ExcelToJsonWindow.DefaultOutputPath);
            var prettyPrint     = EditorPrefs.GetBool(ExcelToJsonWindow.PrettyPrintKey, true);
            var generateClass   = EditorPrefs.GetBool(ExcelToJsonWindow.GenerateClassKey, false);
            var classOutputPath = EditorPrefs.GetString(ExcelToJsonWindow.ClassOutputPathKey, ExcelToJsonWindow.DefaultClassPath);
            var namespaceName   = EditorPrefs.GetString(ExcelToJsonWindow.NamespaceKey, ExcelToJsonWindow.DefaultNamespace);
            var generateManager = EditorPrefs.GetBool(ExcelToJsonWindow.GenerateManagerKey, false);
            var jsonPrefix      = GetJsonResourcePrefix(outputPath);

            bool anyConverted = false;

            foreach (var assetPath in importedAssets)
            {
                if (!assetPath.EndsWith(".xlsx"))
                    continue;

                var fullPath = Path.GetFullPath(assetPath);

                try
                {
                    var results = ExcelToJsonConverter.Convert(fullPath, outputPath, prettyPrint);

                    int success = 0;
                    foreach (var r in results)
                    {
                        if (r.Success) success++;
                        else           Debug.LogError($"[ExcelAutoConverter] '{r.SheetName}' 변환 실패 — {r.Error}");
                    }

                    if (success > 0)
                        Debug.Log($"[ExcelAutoConverter] {Path.GetFileName(assetPath)} → {success}개 시트 변환 완료 ({outputPath})");

                    if (generateClass)
                    {
                        var classResults = ExcelClassGenerator.GenerateFromFile(fullPath, classOutputPath, namespaceName, generateManager, jsonPrefix);

                        int classSuccess = 0;
                        foreach (var r in classResults)
                        {
                            if (r.Success) classSuccess++;
                            else           Debug.LogError($"[ExcelAutoConverter] '{r.SheetName}' 클래스 생성 실패 — {r.Error}");
                        }

                        if (classSuccess > 0)
                            Debug.Log($"[ExcelAutoConverter] {classSuccess}개 클래스 생성 완료 ({classOutputPath})");
                    }

                    anyConverted = success > 0;
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"[ExcelAutoConverter] {Path.GetFileName(assetPath)} 변환 실패: {e.Message}");
                }
            }

            if (anyConverted)
                AssetDatabase.Refresh();
        }

        #endregion

        #region Private Methods

        private static string GetJsonResourcePrefix(string outputPath)
        {
            const string resourcesMarker = "Resources/";
            var idx = outputPath.IndexOf(resourcesMarker, System.StringComparison.Ordinal);
            return idx >= 0 ? outputPath.Substring(idx + resourcesMarker.Length) : outputPath;
        }

        #endregion
    }
}
