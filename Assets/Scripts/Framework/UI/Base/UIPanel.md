# UIPanel

UIManager가 관리하는 UI 패널의 추상 기반 클래스입니다.

---

## 개요

Canvas 단위로 패널을 구성하며, Canvas와 GraphicRaycaster의 활성화/비활성화를 자동으로 처리합니다.
모든 UI 패널은 이 클래스를 상속받아 `OnOpened()` / `OnClosed()`를 override하여 구현합니다.

`IInitializable<TData>`를 함께 구현하면 `UIManager.Open<T, TData>(data)`로 열릴 때 데이터를 전달받을 수 있습니다.

---

## 의존성

없음

---

## API 레퍼런스

### 클래스 / 인터페이스

| 이름 | 설명 |
|------|------|
| `UIPanel` | UI 패널 추상 기반 클래스 |
| `IInitializable<TData>` | 열릴 때 데이터를 전달받는 패널 인터페이스 |

### UIPanel 프로퍼티

| 프로퍼티 | 타입 | 설명 |
|----------|------|------|
| `IsOpen` | `bool` | 패널 열림 여부 (UIPanel 내부 관리) |
| `CanClose` | `virtual bool` | false이면 UIManager.Close/CloseAll에서 건너뜀 (기본값: true) |
| `CloseOnBack` | `virtual bool` | false이면 UIManager.HandleBack에서 건너뜀 (기본값: true) |
| `DestroyOnClose` | `virtual bool` | true이면 닫힐 때 GameObject 파괴 및 레지스트리 제거 (기본값: false) |

### UIPanel 메서드

| 메서드 | 설명 |
|--------|------|
| `OnOpen()` | UIManager에 의해 호출됨. Canvas/Raycaster 활성화 후 OnOpened() 호출 |
| `OnClose()` | UIManager에 의해 호출됨. Canvas/Raycaster 비활성화 후 OnClosed() 호출 |
| `OnOpened()` | `protected virtual` — 등장 연출, 초기화 로직을 구현하세요 |
| `OnClosed()` | `protected virtual` — 퇴장 연출, 정리 로직을 구현하세요 |

### IInitializable\<TData\>

| 멤버 | 설명 |
|------|------|
| `Initialize(TData data)` | Open 시 UIManager에 의해 호출됨. data가 null이면 호출되지 않음 |

---

## 사용법

### 기본 패널 구현

```csharp
using Framework.UI;

// [RequireComponent(Canvas, GraphicRaycaster)]가 자동 적용됩니다.
public class InventoryPanel : UIPanel
{
    protected override void OnOpened()
    {
        // 등장 연출, 데이터 갱신
    }

    protected override void OnClosed()
    {
        // 퇴장 연출, 정리
    }
}
```

### 데이터 전달이 필요한 패널

```csharp
using Framework.UI;

public class ItemDetailPanel : UIPanel, IInitializable<ItemData>
{
    public void Initialize(ItemData data)
    {
        // data를 이용한 초기화
    }

    protected override void OnOpened() { }
    protected override void OnClosed() { }
}

// 사용
UIManager.Instance.Open<ItemDetailPanel, ItemData>(itemData);
```

### 닫기 제어 override

```csharp
public class ConfirmPopup : UIPanel
{
    // Close()/CloseAll()/HandleBack() 모두 무시
    public override bool CanClose => false;
    public override bool CloseOnBack => false;

    protected override void OnOpened() { }
    protected override void OnClosed() { }

    public void OnConfirm() => OnClose();
    public void OnCancel() => OnClose();
}
```

### 한 번만 사용하는 패널 (닫으면 파괴)

```csharp
public class OnboardingPanel : UIPanel
{
    public override bool DestroyOnClose => true;

    protected override void OnOpened() { }
    protected override void OnClosed() { }
}
```

---

## 주의사항

- `Awake()`를 override할 경우 반드시 `base.Awake()`를 호출해야 Canvas/Raycaster가 초기화됩니다.
- `OnOpen()` / `OnClose()`는 UIManager가 호출합니다. 직접 호출하지 마세요. 대신 `UIManager.Instance.Open<T>()` / `UIManager.Instance.Close()`를 사용하세요.
- `IsOpen`은 UIPanel이 내부적으로 관리합니다. 직접 설정하지 마세요.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-03 | 최초 작성 |
