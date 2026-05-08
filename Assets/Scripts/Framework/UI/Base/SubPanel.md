# SubPanel

UIPanel 내부에 배치되는 서브 UI 구획의 추상 기반 클래스.

---

## 개요

### UIPanel vs SubPanel

`UIPanel`은 Canvas를 독립적으로 소유하고 UIManager가 스택/오버레이로 관리하는 **패널 단위**입니다.

`SubPanel`은 부모 UIPanel의 Canvas를 공유하며 UIManager가 관리하지 않는 **하위 UI 구획**입니다. 탭 내 콘텐츠, 미니맵, 스탯 위젯처럼 특정 UIPanel 안에서만 의미 있는 구획을 분리할 때 사용합니다.

| | UIPanel | SubPanel |
|---|---|---|
| Canvas | 자체 보유 | 부모 UIPanel 공유 |
| 관리 주체 | UIManager | 부모 UIPanel |
| 열기/닫기 | `OnOpen()` / `OnClose()` | `Show()` / `Hide()` |
| 데이터 갱신 | 재열기 또는 직접 호출 | `Refresh()` |
| 초기 상태 | Canvas 비활성 | GameObject 비활성 |

### 언제 SubPanel을 쓰나

- 하나의 UIPanel 안에서 **독립적으로 show/hide** 되어야 하는 구획이 있을 때
- 여러 UIPanel에서 **재사용**되는 공통 UI 위젯을 만들 때
- 탭 구조처럼 **같은 영역을 여러 뷰가 공유**할 때

MonoBehaviour를 직접 쓰는 것과의 차이: `IsVisible`, `Show/Hide`, `Refresh` 인터페이스를 통일해 부모 UIPanel이 서브 패널들을 일관된 방식으로 제어할 수 있습니다.

---

## 의존성

- 없음

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `SubPanel` | 서브 패널 추상 기반 클래스 |

### 프로퍼티

| 프로퍼티 | 타입 | 설명 |
|----------|------|------|
| `IsVisible` | `bool` | 현재 표시 중인지 여부 |

### 메서드

| 메서드 | 설명 |
|--------|------|
| `Show()` | SubPanel을 표시하고 `OnShown()` 호출 |
| `Hide()` | SubPanel을 숨기고 `OnHidden()` 호출 |
| `Refresh()` | `OnRefresh()` 호출 — 데이터 변경 시 부모가 호출 |

### 오버라이드 포인트

| 메서드 | 설명 |
|--------|------|
| `OnShown()` | `Show()` 직후 호출. 등장 연출·초기화 |
| `OnHidden()` | `Hide()` 직후 호출. 퇴장 연출·정리 |
| `OnRefresh()` | `Refresh()` 호출 시. 데이터 갱신 로직 |
| `Awake()` | 기본 구현이 `gameObject.SetActive(false)` — 초기 상태 변경 시 override |

---

## 사용법

```csharp
// 1. SubPanel 상속
public class MinimapSubPanel : SubPanel
{
    protected override void OnShown()
    {
        // 미니맵 초기화
    }

    protected override void OnRefresh()
    {
        // dot 위치 갱신
    }
}

// 2. 부모 UIPanel에서 SerializeField로 참조
public class MainPanel : UIPanel
{
    [SerializeField] private MinimapSubPanel _minimap;

    protected override void OnOpened()
    {
        _minimap.Show();
    }

    protected override void OnClosed()
    {
        _minimap.Hide();
    }
}
```

---

## 예시 (심화)

탭 구조에서 탭 전환 시 SubPanel show/hide:

```csharp
public class EquipmentPanel : UIPanel
{
    [SerializeField] private StatsSubPanel  _statsPanel;
    [SerializeField] private SkillSubPanel  _skillPanel;

    private SubPanel _currentTab;

    protected override void OnOpened()
    {
        SwitchTab(_statsPanel);
    }

    public void OnClickStatsTab() => SwitchTab(_statsPanel);
    public void OnClickSkillTab() => SwitchTab(_skillPanel);

    private void SwitchTab(SubPanel next)
    {
        _currentTab?.Hide();
        _currentTab = next;
        _currentTab.Show();
    }
}
```

데이터 변경 시 Refresh 호출:

```csharp
// 스탯이 변경될 때마다 SubPanel에 갱신 요청
EventBus.Subscribe<StatChangedEvent>(_ => _statsPanel.Refresh());
```

---

## 주의사항

- `SubPanel`은 `UIManager`에 등록되지 않습니다. `UIManager.Open/Close`로 제어할 수 없습니다.
- 기본 `Awake()`가 `gameObject.SetActive(false)`를 호출합니다. 처음부터 보여야 하는 경우 `Awake()`를 override하거나 `Show()`를 명시적으로 호출하세요.
- `Refresh()`는 `IsVisible` 여부에 관계없이 호출됩니다. `OnRefresh()` 안에서 `IsVisible`을 확인하고 처리하거나, 숨겨진 상태에서 갱신해도 무방하도록 설계하세요.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-05-08 | 최초 작성 — Show/Hide/Refresh 기반 서브 패널 추상 클래스 |
