using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Core.DataParser.Editor
{
    /// <summary>
    /// Assets 폴더 내 .xlsx 파일이 변경될 때 자동으로 JSON 변환을 수행합니다.
    /// 자동 변환 ON/OFF는 ExcelToJsonWindow의 출력 설정에서 제어합니다.
    /// </summary>
    public class ExcelAutoConverter : AssetPostprocessor
    {
        #region Constants

        internal const string AutoConvertKey = "ExcelToJson_AutoConvert";

        #endregion

        #region Properties

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

            var outputPath = EditorPrefs.GetString(ExcelToJsonWindow.OutputPathKey, ExcelToJsonWindow.DefaultOutputPath);
            var prettyPrint = EditorPrefs.GetBool(ExcelToJsonWindow.PrettyPrintKey, true);

            bool anyConverted = false;

            foreach (var assetPath in importedAssets)
            {
                if (!assetPath.EndsWith(".xlsx"))
                    continue;

                var fullPath = Path.GetFullPath(assetPath);

                try
                {
                    var results = ExcelToJsonConverter.Convert(fullPath, outputPath, prettyPrint);

                    int success = 0, fail = 0;
                    foreach (var r in results)
                    {
                        if (r.Success) success++;
                        else           fail++;
                    }

                    if (success > 0)
                        Debug.Log($"[ExcelAutoConverter] {Path.GetFileName(assetPath)} → {success}개 시트 변환 완료 ({outputPath})");

                    foreach (var r in results)
                    {
                        if (!r.Success)
                            Debug.LogError($"[ExcelAutoConverter] '{r.SheetName}' 변환 실패 — {r.Error}");
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
    }
}
