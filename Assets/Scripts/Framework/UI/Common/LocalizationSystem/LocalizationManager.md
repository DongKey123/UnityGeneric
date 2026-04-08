# LocalizationManager

다국어 텍스트를 런타임에 교체하는 싱글톤 매니저입니다.

---

## 개요

언어 JSON 파일을 `Resources/Localization/` 경로에 저장해두면, `SetLanguage()`로 언어를 변경할 때 자동으로 로드합니다.
`LocalizedText` 컴포넌트를 TMP_Text에 붙이면 언어 변경 시 자동으로 텍스트가 갱신됩니다.

---

## 의존성

- `Framework.Core.Singleton` — `PersistentMonoSingleton<T>` 상속
- `com.unity.nuget.newtonsoft-json` — JSON 파싱

---

## 데이터 형식

```
Assets/Resources/Localization/
├── ko.json
└── en.json
```

```json
{
  "ui_confirm": "확인",
  "ui_cancel": "취소",
  "ui_back": "뒤로",
  "item_sword": "검"
}
```

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `LocalizationManager` | 다국어 텍스트 관리 싱글톤 |
| `LocalizedText` | TMP_Text 자동 갱신 컴포넌트 |

### LocalizationManager 이벤트

| 이벤트 | 타입 | 설명 |
|--------|------|------|
| `OnLanguageChanged` | `Action` | 언어 변경 시 발생 |

### LocalizationManager 프로퍼티

| 프로퍼티 | 타입 | 설명 |
|----------|------|------|
| `CurrentLanguage` | `string` | 현재 적용된 언어 코드 |

### LocalizationManager 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `SetLanguage(languageCode)` | `void` | 언어 변경 및 OnLanguageChanged 발생 |
| `Get(key)` | `string` | 키에 해당하는 텍스트 반환. 없으면 키 자체 반환 |
| `HasKey(key)` | `bool` | 키 존재 여부 확인 |

### LocalizedText 메서드

| 메서드 | 설명 |
|--------|------|
| `SetKey(key)` | 키를 런타임에 변경하고 즉시 갱신 |

---

## 사용법

### 언어 변경

```csharp
// 앱 시작 시 기기 언어에 맞게 설정
string lang = Application.systemLanguage == SystemLanguage.Korean ? "ko" : "en";
LocalizationManager.Instance.SetLanguage(lang);

// 설정 화면에서 수동 변경
LocalizationManager.Instance.SetLanguage("en");
```

### 텍스트 조회

```csharp
string text = LocalizationManager.Instance.Get("ui_confirm"); // "확인"
```

### LocalizedText 컴포넌트

TMP_Text가 붙은 GameObject에 `LocalizedText`를 추가하고 Inspector에서 Key를 지정합니다.
언어가 변경되면 자동으로 텍스트가 갱신됩니다.

```csharp
// 런타임에 키 변경
localizedText.SetKey("item_sword");
```

### 이벤트 구독

```csharp
LocalizationManager.Instance.OnLanguageChanged += OnLangChanged;

private void OnLangChanged()
{
    // 폰트 교체, 레이아웃 재계산 등
}
```

---

## 주의사항

- 언어 파일이 없으면 `FallbackLanguage("en")`로 자동 대체합니다. `en.json`은 반드시 존재해야 합니다.
- `LocalizedText`는 `OnEnable` 시 구독하고 `OnDisable` 시 구독 해제합니다. GameObject가 비활성화된 동안에는 갱신되지 않으며, 활성화될 때 즉시 갱신됩니다.
- `Get()`은 키가 없으면 키 자체를 반환하므로, 빌드 전 누락된 키가 없는지 확인하세요.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-08 | 최초 작성 (SetLanguage / Get / LocalizedText 자동 갱신) |
