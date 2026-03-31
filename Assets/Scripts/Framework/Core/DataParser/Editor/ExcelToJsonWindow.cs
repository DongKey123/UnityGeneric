using System.Collections.Generic;
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
        #region Constants

        internal const string OutputPathKey     = "ExcelToJson_OutputPath";
        internal const string DefaultOutputPath  = "Assets/Resources/Data";
        internal const string PrettyPrintKey    = "ExcelToJson_PrettyPrint";

        #endregion

        #region Fields

        // 변환 설정
        private string          _excelPath      = string.Empty;
        private string          _lastExcelPath  = string.Empty;
        private string          _folderPath     = string.Empty;
        private List<string>    _sheetNames     = new List<string>();
        private HashSet<string> _selectedSheets = new HashSet<string>();
        private Vector2         _sheetScrollPos;

        // 출력 설정
        private string _outputPath  = DefaultOutputPath;
        private bool   _prettyPrint = true;
        private bool   _autoConvert = false;

        // 변환 결과
        private List<SheetConvertResult> _results;
        private int     _previewIndex;
        private Vector2 _resultScrollPos;
        private Vector2 _previewScrollPos;

        // UI 상태
        private bool _showGuide = false;

        #endregion

        #region Menu

        [MenuItem("Framework/Excel To Json")]
        public static void ShowWindow()
        {
            GetWindow<ExcelToJsonWindow>("Excel To Json");
        }

        #endregion

        #region Unity Callbacks

        private void OnEnable()
        {
            _outputPath  = EditorPrefs.GetString(OutputPathKey, DefaultOutputPath);
            _prettyPrint = EditorPrefs.GetBool(PrettyPrintKey, true);
            _autoConvert = ExcelAutoConverter.IsEnabled;
        }

        #endregion

        #region GUI

        private void OnGUI()
        {
            DrawHeader();
            DrawFormatGuide();
            GUILayout.Space(6);
            DrawConvertSettings();
            GUILayout.Space(6);
            DrawOutputSettings();
            DrawConversionResults();
        }

        // ───────────────────────────────────────────────
        //  Header
        // ───────────────────────────────────────────────
        private void DrawHeader()
        {
            GUILayout.Space(6);
            GUILayout.Label("Excel  →  JSON  Converter", EditorStyles.largeLabel);
            DrawSeparator();
        }

        // ───────────────────────────────────────────────
        //  Format Guide (foldout)
        // ───────────────────────────────────────────────
        private void DrawFormatGuide()
        {
            _showGuide = EditorGUILayout.Foldout(_showGuide, "Excel 형식 규칙", true, EditorStyles.foldoutHeader);

            if (!_showGuide)
                return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox(
                "Row 1 : 컬럼 이름   (id, name, attack ...)\n" +
                "Row 2 : 데이터 타입  (int / float / bool / string / enum / int[] / float[] / string[])\n" +
                "Row 3+: 데이터\n\n" +
                "  #  로 시작하는 행       →  주석 (무시)\n" +
                "  #  로 시작하는 시트명   →  무시\n" +
                "  빈 행                   →  무시\n" +
                "  배열 구분자: |          →  예) 10|20|30\n" +
                "  enum 타입               →  string과 동일하게 처리",
                MessageType.Info
            );
            EditorGUILayout.EndVertical();
        }

        // ───────────────────────────────────────────────
        //  변환 설정 (단일 + 폴더)
        // ───────────────────────────────────────────────
        private void DrawConvertSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("변환 설정", EditorStyles.boldLabel);
            GUILayout.Space(4);

            // 단일 파일
            GUILayout.Label("단일 파일", EditorStyles.miniBoldLabel);
            DrawPathRow("Excel 파일", ref _excelPath, () =>
            {
                var p = EditorUtility.OpenFilePanel("Excel 파일 선택", "", "xlsx");
                if (!string.IsNullOrEmpty(p)) _excelPath = p;
            });

            bool singleValid = !string.IsNullOrEmpty(_excelPath) && File.Exists(_excelPath);

            if (!singleValid && !string.IsNullOrEmpty(_excelPath))
                EditorGUILayout.HelpBox("Excel 파일을 찾을 수 없습니다.", MessageType.Error);

            if (singleValid)
            {
                RefreshSheetNamesIfNeeded();
                DrawSheetSelector();
            }

            GUILayout.Space(2);
            GUI.enabled = singleValid;

            if (GUILayout.Button("Convert", GUILayout.Height(26)))
            {
                _results      = ExcelToJsonConverter.Convert(_excelPath, _outputPath, _prettyPrint, _selectedSheets);
                _previewIndex = 0;
                AssetDatabase.Refresh();
            }

            GUI.enabled = true;

            DrawSeparator();

            // 폴더 일괄
            GUILayout.Label("폴더 일괄", EditorStyles.miniBoldLabel);
            DrawPathRow("Excel 폴더", ref _folderPath, () =>
            {
                var p = EditorUtility.OpenFolderPanel("Excel 폴더 선택", "", "");
                if (!string.IsNullOrEmpty(p)) _folderPath = p;
            });

            GUILayout.Space(2);

            bool folderValid = !string.IsNullOrEmpty(_folderPath) && Directory.Exists(_folderPath);
            GUI.enabled = folderValid;

            if (GUILayout.Button("폴더 변환", GUILayout.Height(26)))
            {
                _results      = ExcelToJsonConverter.ConvertFolder(_folderPath, _outputPath, _prettyPrint);
                _previewIndex = 0;
                AssetDatabase.Refresh();
            }

            GUI.enabled = true;

            if (!folderValid && !string.IsNullOrEmpty(_folderPath))
                EditorGUILayout.HelpBox("폴더를 찾을 수 없습니다.", MessageType.Error);

            EditorGUILayout.EndVertical();
        }

        // ───────────────────────────────────────────────
        //  출력 설정
        // ───────────────────────────────────────────────
        private void DrawOutputSettings()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("출력 설정", EditorStyles.boldLabel);
            GUILayout.Space(2);

            DrawOutputPathField();
            GUILayout.Space(4);

            var newPrettyPrint = EditorGUILayout.Toggle("Pretty Print", _prettyPrint);
            if (newPrettyPrint != _prettyPrint)
            {
                _prettyPrint = newPrettyPrint;
                EditorPrefs.SetBool(PrettyPrintKey, _prettyPrint);
            }
            EditorGUILayout.LabelField(
                _prettyPrint
                    ? "  ON  — 들여쓰기 적용 (개발 / 디버깅)"
                    : "  OFF — 압축 출력 (빌드 / 배포)",
                EditorStyles.miniLabel
            );

            GUILayout.Space(4);

            var newAutoConvert = EditorGUILayout.Toggle("자동 변환", _autoConvert);
            if (newAutoConvert != _autoConvert)
            {
                _autoConvert = newAutoConvert;
                ExcelAutoConverter.IsEnabled = _autoConvert;
            }
            EditorGUILayout.LabelField(
                "  Assets 폴더 내 .xlsx 변경 시 자동으로 JSON 변환",
                EditorStyles.miniLabel
            );

            EditorGUILayout.EndVertical();
        }

        // ───────────────────────────────────────────────
        //  변환 결과
        // ───────────────────────────────────────────────
        private void DrawConversionResults()
        {
            if (_results == null || _results.Count == 0)
                return;

            GUILayout.Space(6);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Label("변환 결과", EditorStyles.boldLabel);
            GUILayout.Space(2);

            // 요약
            int successCount = 0, failCount = 0;
            foreach (var r in _results)
            {
                if (r.Success) successCount++;
                else           failCount++;
            }

            string summary = failCount > 0
                ? $"✅ {successCount}개 성공   ❌ {failCount}개 실패"
                : $"✅ {successCount}개 시트 모두 성공";

            GUILayout.Label(summary, EditorStyles.boldLabel);
            GUILayout.Space(4);

            // 시트별 로그
            float logHeight = Mathf.Min(_results.Count * 20f, 100f);
            _resultScrollPos = EditorGUILayout.BeginScrollView(_resultScrollPos, GUILayout.Height(logHeight));

            foreach (var r in _results)
            {
                EditorGUILayout.LabelField(r.Success
                    ? $"✅  {r.SheetName}  ({r.RowCount} rows)"
                    : $"❌  {r.SheetName}  →  {r.Error}");
            }

            EditorGUILayout.EndScrollView();

            // JSON 미리보기 (성공한 시트만)
            var successList = new List<SheetConvertResult>();
            foreach (var r in _results)
            {
                if (r.Success) successList.Add(r);
            }

            if (successList.Count == 0)
            {
                EditorGUILayout.EndVertical();
                return;
            }

            DrawSeparator();
            GUILayout.Label("JSON 미리보기", EditorStyles.miniBoldLabel);

            var sheetNames = new string[successList.Count];
            for (int i = 0; i < successList.Count; i++)
                sheetNames[i] = successList[i].SheetName;

            _previewIndex = Mathf.Clamp(_previewIndex, 0, successList.Count - 1);
            _previewIndex = EditorGUILayout.Popup(_previewIndex, sheetNames);

            GUILayout.Space(2);

            var previewJson = successList[_previewIndex].Json ?? string.Empty;
            _previewScrollPos = EditorGUILayout.BeginScrollView(_previewScrollPos, GUILayout.Height(160));
            EditorGUILayout.SelectableLabel(previewJson, EditorStyles.textArea, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();
        }

        // ───────────────────────────────────────────────
        //  Helpers
        // ───────────────────────────────────────────────
        private void DrawOutputPathField()
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("출력 폴더", GUILayout.Width(62));

            var newPath = EditorGUILayout.TextField(_outputPath);
            if (newPath != _outputPath)
            {
                _outputPath = newPath;
                EditorPrefs.SetString(OutputPathKey, _outputPath);
            }

            if (GUILayout.Button("...", GUILayout.Width(28)))
            {
                var path = EditorUtility.OpenFolderPanel("출력 폴더 선택", "Assets", "");
                if (!string.IsNullOrEmpty(path))
                {
                    _outputPath = "Assets" + path.Substring(Application.dataPath.Length);
                    EditorPrefs.SetString(OutputPathKey, _outputPath);
                    GUI.FocusControl(null);
                }
            }

            if (GUILayout.Button("Default", GUILayout.Width(52)))
            {
                _outputPath = DefaultOutputPath;
                EditorPrefs.SetString(OutputPathKey, _outputPath);
                GUI.FocusControl(null);
            }

            GUILayout.EndHorizontal();
        }

        private void RefreshSheetNamesIfNeeded()
        {
            if (_excelPath == _lastExcelPath)
                return;

            _lastExcelPath = _excelPath;

            try
            {
                _sheetNames     = ExcelToJsonConverter.GetSheetNames(_excelPath);
                _selectedSheets = new HashSet<string>(_sheetNames);
            }
            catch
            {
                _sheetNames     = new List<string>();
                _selectedSheets = new HashSet<string>();
            }
        }

        private void DrawSheetSelector()
        {
            GUILayout.Space(4);

            GUILayout.BeginHorizontal();
            GUILayout.Label("시트 선택", EditorStyles.miniBoldLabel);
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("전체 선택", EditorStyles.miniButton, GUILayout.Width(72)))
                _selectedSheets = new HashSet<string>(_sheetNames);

            if (GUILayout.Button("전체 해제", EditorStyles.miniButton, GUILayout.Width(72)))
                _selectedSheets.Clear();

            GUILayout.EndHorizontal();

            float scrollHeight = Mathf.Min(_sheetNames.Count * 20f, 100f);
            _sheetScrollPos = EditorGUILayout.BeginScrollView(_sheetScrollPos, GUILayout.Height(scrollHeight));

            foreach (var sheet in _sheetNames)
            {
                bool isSelected = _selectedSheets.Contains(sheet);
                bool toggled    = EditorGUILayout.ToggleLeft(sheet, isSelected);

                if (toggled != isSelected)
                {
                    if (toggled) _selectedSheets.Add(sheet);
                    else         _selectedSheets.Remove(sheet);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private static void DrawPathRow(string label, ref string value, System.Action onBrowse)
        {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(62));
            value = EditorGUILayout.TextField(value);

            if (GUILayout.Button("...", GUILayout.Width(28)))
                onBrowse();

            GUI.enabled = !string.IsNullOrEmpty(value);
            if (GUILayout.Button("✕", GUILayout.Width(22)))
                value = string.Empty;
            GUI.enabled = true;

            GUILayout.EndHorizontal();
        }

        private static void DrawSeparator()
        {
            GUILayout.Space(2);
            EditorGUILayout.LabelField(string.Empty, GUI.skin.horizontalSlider);
            GUILayout.Space(2);
        }

        #endregion
    }
}
