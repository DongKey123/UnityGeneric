# CommonPopupManager

OneButtonPopup / TwoButtonPopup 등 공용 팝업을 스택으로 관리하는 싱글톤입니다.

---

## 개요

UIManager와 별개로 팝업 전용 레이어를 관리합니다.

- 팝업이 열릴 때 Dim(반투명 배경) 오버레이를 자동으로 표시하고, 모든 팝업이 닫히면 숨깁니다.
- `OneButtonPopup`, `TwoButtonPopup` 프리팹을 `Resources/UI/` 경로에 저장하면 자동으로 로드 및 인스턴스화합니다.
- 팝업의 버튼 콜백은 Close 액션을 주입받아 처리하므로, 팝업 클래스가 CommonPopupManager에 직접 의존하지 않습니다.

---

## 의존성

- `Framework.Core.Singleton` — `PersistentMonoSingleton<T>` 상속

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `CommonPopupManager` | 공용 팝업 관리 싱글톤 |
| `PopupBase` | 팝업 추상 기반 클래스 |
| `OneButtonPopup` | 확인 버튼 1개 팝업 |
| `TwoButtonPopup` | 확인/취소 버튼 2개 팝업 |

### CommonPopupManager 프로퍼티

| 프로퍼티 | 타입 | 설명 |
|----------|------|------|
| `PopupCount` | `int` | 현재 열려 있는 팝업 수 |
| `IsAnyOpen` | `bool` | 팝업이 하나 이상 열려 있는지 여부 |

### CommonPopupManager 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `ShowOneButton(title, message, buttonText, onConfirm)` | `void` | 확인 버튼 1개 팝업 표시 |
| `ShowTwoButton(title, message, confirmText, cancelText, onConfirm, onCancel)` | `void` | 확인/취소 버튼 2개 팝업 표시 |
| `Close()` | `void` | 최상단 팝업 닫기 |
| `CloseAll()` | `void` | 모든 팝업 닫기 |
| `HandleBack()` | `bool` | 뒤로가기 처리. 닫은 팝업이 있으면 true 반환 |

### PopupBase 멤버

| 멤버 | 설명 |
|------|------|
| `IsOpen` | 팝업 열림 여부 (PopupBase 내부 관리) |
| `OnOpen()` | CommonPopupManager에 의해 호출됨. SetActive(true) 후 OnOpened() 호출 |
| `OnClose()` | CommonPopupManager에 의해 호출됨. SetActive(false) 후 OnClosed() 호출 |
| `OnOpened()` | `protected virtual` — 서브클래스에서 override |
| `OnClosed()` | `protected virtual` — 서브클래스에서 override |

---

## 사용법

### 버튼 1개 팝업

```csharp
CommonPopupManager.Instance.ShowOneButton(
    title: "알림",
    message: "저장이 완료되었습니다.",
    buttonText: "확인",
    onConfirm: () => Debug.Log("확인 클릭")
);
```

### 버튼 2개 팝업

```csharp
CommonPopupManager.Instance.ShowTwoButton(
    title: "아이템 삭제",
    message: "정말 삭제하시겠습니까?",
    confirmText: "삭제",
    cancelText: "취소",
    onConfirm: () => DeleteItem(),
    onCancel: () => Debug.Log("취소")
);
```

### 뒤로가기 연동

뒤로가기 입력은 `InputManager`를 상속받아 `HandleBack()`을 override하여 처리하세요.
CommonPopupManager는 입력 코드를 직접 갖지 않습니다.

```csharp
// 게임 코드 예시 (프레임워크 외부)
public class GameInputManager : MobileInputManager
{
    protected override void HandleBack()
    {
        // 팝업 먼저, 없으면 UIManager
        if (CommonPopupManager.Instance.HandleBack()) return;
        UIManager.Instance.HandleBack();
    }
}
```

### 팝업 위에 팝업 쌓기

```csharp
CommonPopupManager.Instance.ShowOneButton("알림", "저장 완료");

// 위 팝업이 열린 상태에서 추가 팝업 표시
// TwoButtonPopup은 별도 타입이므로 동시에 열 수 있음
CommonPopupManager.Instance.ShowTwoButton("확인", "계속하시겠습니까?", onConfirm: Continue);
```

---

---

## 인스펙터 설정

| 필드 | 설명 |
|------|------|
| `Popup Canvas` | 팝업이 생성될 Canvas. 미할당 시 CommonPopupManager GameObject 하위에 생성 |
| `Dim` | 팝업 뒤 반투명 배경 CanvasGroup. 미할당 시 Dim 처리 생략 |

**Popup Canvas Sort Order**는 UIManager의 Canvas보다 높게 설정해야 팝업이 항상 UI 위에 표시됩니다.

---

## 주의사항

- 같은 타입의 팝업이 이미 열려 있는 상태에서 같은 타입을 다시 열면 경고 로그를 출력하고 무시합니다.
- `OneButtonPopup`과 `TwoButtonPopup`은 각각 인스턴스가 1개만 유지됩니다. 동시에 같은 타입 두 개가 필요하면 커스텀 팝업을 구현하세요.
- 팝업 프리팹은 `Resources/UI/OneButtonPopup`, `Resources/UI/TwoButtonPopup` 경로에 저장해야 합니다.
- `PopupBase.Awake()`를 override할 경우 반드시 `base.Awake()`를 호출해야 초기 비활성화가 처리됩니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-08 | 최초 작성 (OneButtonPopup / TwoButtonPopup / Dim / 스택 관리) |
