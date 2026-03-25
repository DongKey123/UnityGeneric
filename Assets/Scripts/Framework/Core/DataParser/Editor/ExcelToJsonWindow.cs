using System.IO;
using UnityEditor;
using UnityEngine;

namespace Framework.Core.DataParser.Editor
{
    /// <summary>
    /// Excel → JSON 변환 EditorWindow.
    /// 메뉴: Framework > Excel To Json
    /// </summary>
    public class ExcelToJsonWindow : EditorWindow
    {
        #region Fields

        private string _excelPath = string.Empty;
        private string _outputPath = "Assets/Resources/Data";
        private bool _prettyPrint = true;

        #endregion

        #region Menu

        [MenuItem("Framework/Excel To Json")]
        public static void ShowWindow()
        {
            GetWindow<ExcelToJsonWindow>("Excel To Json");
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            GUILayout.Label("Excel To Json Converter", EditorStyles.boldLabel);
            GUILayout.Space(8);

            DrawExcelPathField();
            DrawOutputPathField();

            _prettyPrint = EditorGUILayout.Toggle("Pretty Print", _prettyPrint);
            EditorGUILayout.HelpBox("ON: JSON에 들여쓰기 적용 (사람이 읽기 편함, 개발/디버깅 용도)\nOFF: JSON 압축 출력 (파일 크기 작음, 빌드/배포 용도)", MessageType.None);

            GUILayout.Space(8);
            DrawConvertButton();

            GUILayout.Space(16);
            DrawFormatGuide();
        }

        private void DrawExcelPathField()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Excel 파일", GUILayout.Width(70));
            _excelPath = EditorGUILayout.TextField(_excelPath);

            if (GUILayout.Button("...", GUILayout.Width(28)))
            {
                var path = EditorUtility.OpenFilePanel("Excel 파일 선택", "", "xlsx");
                if (!string.IsNullOrEmpty(path))
                    _excelPath = path;
            }

            GUILayout.EndHorizontal();
        }

        private void DrawOutputPathField()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("출력 폴더", GUILayout.Width(70));
            _outputPath = EditorGUILayout.TextField(_outputPath);

            if (GUILayout.Button("...", GUILayout.Width(28)))
            {
                var path = EditorUtility.OpenFolderPanel("출력 폴더 선택", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                    _outputPath = "Assets" + path.Substring(Application.dataPath.Length);
            }

            GUILayout.EndHorizontal();
        }

        private void DrawConvertButton()
        {
            bool isValid = !string.IsNullOrEmpty(_excelPath) && File.Exists(_excelPath);

            GUI.enabled = isValid;

            if (GUILayout.Button("Convert", GUILayout.Height(30)))
            {
                int count = ExcelToJsonConverter.Convert(_excelPath, _outputPath, _prettyPrint);
                AssetDatabase.Refresh();
                EditorUtility.DisplayDialog("완료", $"{count}개 시트를 변환했습니다.\n출력 경로: {_outputPath}", "확인");
            }

            GUI.enabled = true;

            if (!isValid && !string.IsNullOrEmpty(_excelPath))
            {
                EditorGUILayout.HelpBox("Excel 파일을 찾을 수 없습니다.", MessageType.Error);
            }
        }

        private void DrawFormatGuide()
        {
            GUILayout.Label("Excel 형식 규칙", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Row 1 : 컬럼 이름 (id, name, attack ...)\n" +
                "Row 2 : 데이터 타입 (int / float / bool / string / int[] / float[] / string[])\n" +
                "Row 3+: 데이터\n\n" +
                "# 로 시작하는 행 → 주석 (무시)\n" +
                "# 로 시작하는 시트 이름 → 무시\n" +
                "빈 행 → 무시\n" +
                "배열 구분자: | (예: 10|20|30)",
                MessageType.Info
            );
        }

        #endregion
    }
}
