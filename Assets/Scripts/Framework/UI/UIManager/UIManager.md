# UIManager

UI 패널을 2레이어(Default 스택 / Overlay)로 관리하는 싱글톤 매니저입니다.

---

## 개요

`UIPanel`을 상속한 패널들을 Canvas 단위로 관리합니다.

- **Default 레이어**: 스택 기반 패널 네비게이션 (메뉴, 설정, 인벤토리 등)
- **Overlay 레이어**: 스택과 무관하게 최상단에 표시되는 패널 (로딩 화면, 공지 등)

패널 프리팹은 `Resources/UI/` 경로에 **클래스 이름과 동일한 이름**으로 저장해야 합니다.
`Open<T>()`이 처음 호출될 때 해당 경로에서 자동으로 로드 및 인스턴스화하며, 이후 `Get<T>()`로 참조 없이 조회할 수 있습니다.

---

## 의존성

- `Framework.Core.Singleton` — `PersistentMonoSingleton<T>` 상속

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `UIManager` | UI 패널 2레이어 관리 싱글톤 |
| `UIPanel` | 패널 추상 기반 클래스 (Canvas 단위, `Framework.UI.Base`) |

### UIPanel (추상 클래스)

`[RequireComponent(Canvas, GraphicRaycaster)]`가 자동 적용됩니다.

| 멤버 | 타입 | 설명 |
|------|------|------|
| `IsOpen` | `bool` | 패널 열림 여부 (UIPanel 내부 관리) |
| `CanClose` | `virtual bool` | false이면 Close/CloseAll에서 건너뜀 (기본값: true) |
| `CloseOnBack` | `virtual bool` | false이면 HandleBack에서 건너뜀 (기본값: true) |
| `DestroyOnClose` | `virtual bool` | true이면 닫힐 때 GameObject 파괴 및 레지스트리 제거 (기본값: false) |
| `OnOpen()` | `void` | Canvas/Raycaster 활성화 후 OnOpened() 호출 |
| `OnClose()` | `void` | Canvas/Raycaster 비활성화 후 OnClosed() 호출 |
| `OnOpened()` | `protected virtual void` | 서브클래스에서 override — 등장 연출, 초기화 |
| `OnClosed()` | `protected virtual void` | 서브클래스에서 override — 퇴장 연출, 정리 |

### UIManager 이벤트

| 이벤트 | 타입 | 설명 |
|--------|------|------|
| `OnPanelOpened` | `Action<UIPanel>` | 패널이 열릴 때 발생 |
| `OnPanelClosed` | `Action<UIPanel>` | 패널이 닫힐 때 발생 |
| `OnStackEmpty` | `Action` | Default 스택이 비었을 때 발생 |

### UIManager 프로퍼티

| 프로퍼티 | 타입 | 설명 |
|----------|------|------|
| `PanelCount` | `int` | Default 스택의 현재 열린 패널 수 |
| `Current` | `UIPanel` | Default 스택 최상단 패널 (없으면 null) |
| `IsOverlayOpen` | `bool` | Overlay 레이어에 열린 패널이 있는지 여부 |

### UIManager 메서드 — Default 레이어

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Open<T>()` | `void` | 패널 열기. 미등록 시 `Resources/UI/`에서 자동 로드 |
| `Open<T, TData>(TData data = null)` | `void` | 데이터를 전달하며 패널 열기. 패널이 `IInitializable<TData>`를 구현해야 함 |
| `Close()` | `void` | 최상단 패널 닫기 (CanClose=false이면 건너뜀) |
| `CloseAll()` | `void` | CanClose=true인 모든 패널 닫기 |
| `HandleBack()` | `bool` | 최상단 패널의 CloseOnBack=true이면 닫기. 닫으면 true, CloseOnBack=false이면 false 반환 |

### UIManager 메서드 — Overlay 레이어

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `ShowOverlay<T>()` | `void` | 오버레이 패널 표시. 미등록 시 `Resources/UI/`에서 자동 로드 |
| `HideOverlay<T>()` | `void` | 특정 타입 오버레이 패널 숨기기 |
| `HideAllOverlays()` | `void` | 모든 오버레이 패널 숨기기 |

### UIManager 메서드 — 리셋

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `ResetAll()` | `void` | 모든 패널 파괴 및 레지스트리 초기화. BootScene 전환 시 호출 |

### UIManager 메서드 — 레지스트리

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Get<T>()` | `T` | 타입으로 등록된 패널 반환 (미등록 시 예외 발생) |
| `IsOpen<T>()` | `bool` | 해당 타입 패널이 열려 있는지 확인 |

---

## 사용법

### 패널 구현

```csharp
using Framework.UI;

// GameObject에 Canvas, GraphicRaycaster가 자동으로 추가됩니다.
public class InventoryPanel : UIPanel
{
    protected override void OnOpened()
    {
        // 등장 연출, 초기화 로직
    }

    protected override void OnClosed()
    {
        // 퇴장 연출, 정리 로직
    }
}
```

### 데이터 전달하며 패널 열기

```csharp
// 데이터를 전달받을 패널은 IInitializable<TData>를 구현합니다.
public class ItemDetailPanel : UIPanel, IInitializable<ItemData>
{
    public void Initialize(ItemData data)
    {
        // data를 이용한 초기화
    }
}

// 데이터와 함께 열기
UIManager.Instance.Open<ItemDetailPanel, ItemData>(itemData);

// null로 열기 (Initialize 호출 안 됨)
UIManager.Instance.Open<ItemDetailPanel, ItemData>();
```

### 패널 열기/닫기

```csharp
// 데이터가 필요 없는 패널 열기
UIManager.Instance.Open<InventoryPanel>();

// 최상단 패널 닫기
UIManager.Instance.Close();

// CanClose=true인 모든 패널 닫기
UIManager.Instance.CloseAll();
```

### 타입 기반 조회

```csharp
// 등록된 패널 가져오기 (미등록 시 예외 발생)
var panel = UIManager.Instance.Get<InventoryPanel>();

// 열려 있는지 확인 (미등록 시 false 반환)
if (UIManager.Instance.IsOpen<InventoryPanel>())
{
    // ...
}
```

### 오버레이 사용

Overlay는 **사용자 응답이 필요하거나 모든 UI 위에 반드시 표시되어야 하는 패널**에 사용합니다.

| 패널 예시 | 설명 |
|-----------|------|
| `LoadingPanel` | 씬 전환 로딩 화면 |
| `NetworkErrorPanel` | 서버 연결 끊김, 재시도 팝업 |
| `SystemNoticePanel` | 점검 공지, 강제 업데이트 안내 |
| `TutorialOverlay` | 전체화면 튜토리얼 가이드 |

> 잠깐 보이고 자동으로 사라지는 알림(레벨업, 아이템 획득 등)은 Overlay가 아닌 **ToastManager**를 사용하세요.

```csharp
// 오버레이 표시
UIManager.Instance.ShowOverlay<LoadingPanel>();

// 오버레이 숨기기
UIManager.Instance.HideOverlay<LoadingPanel>();
```

### 이벤트 구독

```csharp
UIManager.Instance.OnPanelOpened += panel => Debug.Log($"열림: {panel.GetType().Name}");
UIManager.Instance.OnStackEmpty += () => Debug.Log("모든 패널 닫힘");
```

### 모바일 뒤로가기 연동

```csharp
private void Update()
{
    if (Input.GetKeyDown(KeyCode.Escape))
    {
        UIManager.Instance.HandleBack();
    }
}
```

---

## 예시 (심화)

### 닫기 방지 팝업 (CanClose=false)

```csharp
public class ConfirmPopup : UIPanel
{
    public override bool CanClose => false;
    public override bool CloseOnBack => false;

    protected override void OnOpened() { }
    protected override void OnClosed() { }

    // 확인/취소 버튼에서 직접 OnClose 호출
    public void OnConfirm() => OnClose();
    public void OnCancel() => OnClose();
}
```

---

## Details

### Canvas per Panel 구조

각 패널은 Canvas를 하나씩 가집니다. Unity는 **Canvas 단위로 re-batch**를 수행하기 때문에, 하나의 Canvas에 모든 패널을 넣으면 버튼 하나만 바뀌어도 전체 UI가 다시 배칭됩니다. 패널마다 Canvas를 분리하면 변경된 패널만 re-batch되어 성능에 유리합니다.

Canvas는 전체 화면을 덮을 필요가 없습니다. 작은 팝업, 알림 UI도 콘텐츠 크기에 맞는 Canvas를 가질 수 있습니다.

---

### UI 텍스처(스프라이트) 폴더 구조

디자이너가 작업한 UI 에셋은 아래 구조로 저장합니다.

```
Assets/
└── Art/
    └── UI/
        ├── Common/          ← 공용 버튼, 프레임, 아이콘 등
        │   ├── btn_confirm.png
        │   ├── btn_cancel.png
        │   └── frame_popup.png
        ├── Inventory/       ← 인벤토리 패널 전용 에셋
        │   ├── bg_inventory.png
        │   └── slot_item.png
        ├── Shop/            ← 상점 패널 전용 에셋
        │   └── bg_shop.png
        └── Overlay/         ← 로딩 화면 등 Overlay 전용 에셋
            └── bg_loading.png
```

**Atlas(Sprite Atlas) 구성 권장**

| Atlas 이름 | 포함 대상 | 설명 |
|---|---|---|
| `Atlas_Common` | `Common/` 전체 | 여러 패널에서 공용으로 사용 |
| `Atlas_Inventory` | `Inventory/` 전체 | 인벤토리 패널 전용 |
| `Atlas_Shop` | `Shop/` 전체 | 상점 패널 전용 |

- Atlas 단위로 Draw Call이 묶이므로 패널별로 Atlas를 분리하면 해당 패널이 열릴 때만 Atlas가 로드됩니다.
- Common Atlas는 항상 메모리에 올려두고, 패널별 Atlas는 패널 Open/Close 시점에 로드/언로드합니다.

---

## 주의사항

- 이미 열려 있는 패널을 `Open()`하면 중복 추가되지 않고 경고 로그를 출력합니다.
- Overlay 패널은 Default 스택의 `PanelCount`, `Current`에 포함되지 않습니다.
- `Get<T>()`는 한 번도 `Open()` / `ShowOverlay()`를 호출하지 않은 패널에 사용하면 예외가 발생합니다. 열림 여부만 확인할 때는 `IsOpen<T>()`를 사용하세요.
- `UIPanel.Awake()`를 override할 경우 반드시 `base.Awake()`를 호출해야 Canvas/Raycaster가 초기화됩니다.
- UIManager 인스펙터에서 `Default Canvas`와 `Overlay Canvas`를 반드시 할당해야 합니다. 미할당 시 패널이 UIManager GameObject 하위에 생성됩니다.

---

## 추가 예정

| 항목 | 설명 | 비고 |
|------|------|------|
| 트랜지션 비동기 지원 | Open/Close 시 페이드/슬라이드 애니메이션을 UIManager가 await할 수 있는 구조 | AsyncHelper(UniTask) 완료 후 작업 예정 |
| Addressables 전환 | Resources.Load 동기 로드를 Addressables 비동기 로드로 교체 | 프로젝트 규모에 따라 도입 여부 결정 |

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 2.0.0 | 2026-04-03 | Canvas 단위 UIPanel 추상 클래스 도입, 2레이어(Default/Overlay) 재설계, CanClose/CloseOnBack/이벤트/레지스트리 추가 |
| 1.0.0 | 2026-04-01 | 최초 작성 |
