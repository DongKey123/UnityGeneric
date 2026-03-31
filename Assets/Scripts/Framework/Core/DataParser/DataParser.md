# DataParser / ExcelToJson

Unity Editor에서 Excel(.xlsx) 파일을 읽어 시트별 JSON 파일로 변환하는 에디터 툴입니다.

---

## 개요

기획 데이터를 Excel로 작성하고 JSON으로 변환하는 워크플로우를 지원합니다.
단일 파일 변환, 폴더 일괄 변환, 자동 변환을 제공하며 변환 결과를 에디터 창 내에서 바로 확인할 수 있습니다.

메뉴: **Framework > Excel To Json**

---

## 의존성

- [ExcelDataReader](https://github.com/ExcelDataReader/ExcelDataReader) — .xlsx 파일 파싱
- [Newtonsoft.Json](https://www.newtonsoft.com/json) — JSON 직렬화

---

## Excel 형식 규칙

| Row | 내용 |
|-----|------|
| 1 | 컬럼 이름 (id, name, attack ...) |
| 2 | 데이터 타입 |
| 3+ | 실제 데이터 |

### 지원 타입

| 타입 | 설명 | 예시 |
|------|------|------|
| `int` | 정수 | `10` |
| `float` | 실수 | `1.5` |
| `bool` | 불리언 | `true` / `1` / `yes` |
| `string` | 문자열 | `슬라임` |
| `enum` | string과 동일하게 처리 | `Warrior` |
| `int[]` | 정수 배열 | `10\|20\|30` |
| `float[]` | 실수 배열 | `1.0\|2.5` |
| `string[]` | 문자열 배열 | `불\|물\|바람` |

### 특수 규칙

- `#` 으로 시작하는 행 → 주석 (무시)
- `#` 으로 시작하는 시트명 → 무시
- 빈 행 → 무시
- 배열 구분자: `|`

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `ExcelToJsonConverter` | Excel → JSON 변환 로직 (static) |
| `ExcelToJsonWindow` | EditorWindow UI |
| `ExcelAutoConverter` | AssetPostprocessor 기반 자동 변환 |
| `SheetConvertResult` | 시트 하나의 변환 결과 데이터 |

### ExcelToJsonConverter 주요 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Convert(excelPath, outputDir, prettyPrint, selectedSheets)` | `List<SheetConvertResult>` | 단일 파일 변환 |
| `ConvertFolder(folderPath, outputDir, prettyPrint)` | `List<SheetConvertResult>` | 폴더 내 전체 .xlsx 변환 |
| `GetSheetNames(excelPath)` | `List<string>` | 시트 이름 목록 반환 (#시트 제외) |

### SheetConvertResult 필드

| 필드 | 타입 | 설명 |
|------|------|------|
| `SheetName` | `string` | 시트 이름 |
| `Success` | `bool` | 변환 성공 여부 |
| `RowCount` | `int` | 변환된 데이터 행 수 |
| `Error` | `string` | 실패 시 원인 메시지 |
| `Json` | `string` | 성공 시 변환된 JSON 문자열 |

---

## 사용법

### 에디터 창 사용

1. **Framework > Excel To Json** 메뉴로 창을 엽니다.
2. **변환 설정** — Excel 파일 또는 폴더를 선택합니다.
3. **출력 설정** — 출력 폴더, Pretty Print, 자동 변환을 설정합니다.
4. Convert / 폴더 변환 버튼을 누르면 **변환 결과** 섹션에서 시트별 성공/실패 로그와 JSON 미리보기를 확인합니다.

### 스크립트에서 직접 호출

```csharp
// 단일 파일 변환
var results = ExcelToJsonConverter.Convert(
    excelPath:      "/path/to/data.xlsx",
    outputDir:      "Assets/Resources/Data",
    prettyPrint:    true,
    selectedSheets: null   // null = 전체 시트
);

foreach (var r in results)
{
    if (r.Success)
        Debug.Log($"{r.SheetName}: {r.RowCount}행 변환 완료");
    else
        Debug.LogError($"{r.SheetName}: {r.Error}");
}
```

### 특정 시트만 변환

```csharp
var selected = new HashSet<string> { "Monster", "Item" };

var results = ExcelToJsonConverter.Convert(
    "/path/to/data.xlsx",
    "Assets/Resources/Data",
    prettyPrint: true,
    selectedSheets: selected
);
```

---

## 예시 (심화)

### Excel 시트 예시

| id | name | hp | drop |
|----|------|----|------|
| int | string | int | int[] |
| 1 | 슬라임 | 100 | 10\|50 |
| 2 | 고블린 | 200 | 20\|30\|100 |

### 출력 JSON (Pretty Print ON)

```json
[
  {
    "id": 1,
    "name": "슬라임",
    "hp": 100,
    "drop": [ 10, 50 ]
  },
  {
    "id": 2,
    "name": "고블린",
    "hp": 200,
    "drop": [ 20, 30, 100 ]
  }
]
```

---

## 주의사항

- Editor 전용 툴입니다. `Editor` 폴더 안에 위치하며 런타임 빌드에 포함되지 않습니다.
- `.xlsx` 형식만 지원합니다. `.xls` / `.csv` 는 지원하지 않습니다.
- 자동 변환은 **Assets 폴더 내** `.xlsx` 파일에만 반응합니다. 외부 경로 파일은 수동 변환이 필요합니다.
- 헤더 행(Row 1)에 빈 셀이 나오면 그 이후 컬럼은 모두 무시됩니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-03-31 | 최초 완성 — 단일/폴더 변환, 시트 선택, 자동 변환, 변환 결과 미리보기 |
