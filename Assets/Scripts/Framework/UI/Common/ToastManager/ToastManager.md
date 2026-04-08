# ToastManager

화면에 잠깐 표시되고 자동으로 사라지는 토스트 메시지를 관리하는 싱글톤입니다.

---

## 개요

UIManager와 독립된 별도 싱글톤입니다.
여러 메시지가 동시에 요청되면 큐에 쌓아 순서대로 표시합니다.
`ToastPanel` 프리팹은 `Resources/UI/ToastPanel`에 저장해야 합니다.

---

## 의존성

- `Framework.Core.Singleton` — `PersistentMonoSingleton<T>` 상속
- TextMeshPro — `TMP_Text` 사용

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `ToastManager` | 토스트 메시지 관리 싱글톤 |
| `ToastPanel` | 토스트 UI 컴포넌트 (페이드 인/아웃) |
| `ToastRequest` | 토스트 요청 데이터 |
| `ToastType` | 토스트 종류 열거형 |

### ToastType

| 값 | 설명 |
|----|------|
| `Default` | 일반 메시지 |
| `Success` | 성공 메시지 |
| `Warning` | 경고 메시지 |
| `Error` | 오류 메시지 |

### ToastManager 메서드

| 메서드 | 설명 |
|--------|------|
| `Show(string message, float duration = 2f)` | 기본 타입 토스트 표시 |
| `Show(string message, ToastType type, float duration = 2f)` | 타입 지정 토스트 표시 |

---

## 사용법

```csharp
// 기본 메시지
ToastManager.Instance.Show("저장되었습니다.");

// 타입 지정
ToastManager.Instance.Show("구매 완료!", ToastType.Success);
ToastManager.Instance.Show("네트워크 오류", ToastType.Error);
ToastManager.Instance.Show("재화가 부족합니다.", ToastType.Warning);

// 표시 시간 지정
ToastManager.Instance.Show("아이템 획득!", ToastType.Success, 3f);
```

---

## Details

### 큐 동작 방식

여러 토스트가 연속으로 요청되면 현재 토스트가 끝난 후 순서대로 표시됩니다.

```
Show("A") → Show("B") → Show("C")
결과: A 표시 → A 사라짐 → B 표시 → B 사라짐 → C 표시
```

### ToastPanel 프리팹 구성

```
ToastPanel (CanvasGroup)
  ├── Background (Image)
  ├── MessageText (TMP_Text)
  └── Icons
        ├── DefaultIcon  (GameObject)
        ├── SuccessIcon  (GameObject)
        ├── WarningIcon  (GameObject)
        └── ErrorIcon    (GameObject)
```

아이콘은 선택사항입니다. Inspector에서 연결하지 않으면 표시되지 않습니다.

### 인스펙터 설정

`ToastManager` GameObject의 Inspector에서 `Toast Canvas`를 할당합니다.
할당하지 않으면 ToastManager GameObject 하위에 생성됩니다.
Sorting Order를 높게 설정하여 다른 UI 위에 표시되도록 하세요.

---

## 주의사항

- `ToastPanel`은 ToastManager가 내부적으로 관리합니다. UIManager를 통해 열지 마세요.
- 토스트는 자동으로 사라지므로 `Close()` 호출이 없습니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-08 | 최초 작성 |
