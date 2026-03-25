# EventBus

오브젝트 간 직접 참조 없이 이벤트를 주고받는 글로벌 이벤트 시스템입니다.

---

## 개요

일반적인 이벤트 연결은 발행자와 구독자가 서로를 직접 참조해야 합니다.
EventBus는 중간 버스를 통해 참조 없이 어디서든 이벤트를 발행하고 구독할 수 있습니다.

```csharp
// 직접 참조 방식 — 결합도 높음
player.OnDead += uiManager.ShowGameOver;

// EventBus 방식 — 결합도 낮음
EventBus.Publish(new PlayerDeadEvent());
EventBus.Subscribe<PlayerDeadEvent>(OnPlayerDead);
```

<details>
<summary>기본 개념 — 발행/구독 패턴이란?</summary>

발행/구독(Pub/Sub) 패턴은 메시지를 보내는 쪽(발행자)과 받는 쪽(구독자)이 서로를 직접 알지 못하도록 분리하는 패턴입니다.

- **발행자**: 이벤트가 발생했음을 버스에 알림
- **버스**: 해당 이벤트를 구독 중인 핸들러에 전달
- **구독자**: 관심 있는 이벤트만 선택적으로 수신

이 구조 덕분에 씬이 달라도, 오브젝트 간 참조가 없어도 이벤트를 주고받을 수 있습니다.

</details>

---

## 이벤트 정의 규칙

EventBus의 이벤트는 `struct` 사용을 권장합니다.

- `class`는 힙 할당으로 GC 부하가 생기지만 `struct`는 스택 할당으로 GC가 발생하지 않습니다.
- `class`도 사용할 수 있으나, 이벤트가 자주 발행되는 경우 GC 부하가 커질 수 있습니다.
- 이벤트는 Framework 코드가 아닌 **프로젝트 폴더에서 직접 정의**합니다.

**권장 폴더 구조**

```
Assets/Scripts/
├── Framework/Core/EventBus/
│   └── EventBus.cs              ← 프레임워크 코드 (수정 불필요)
│
└── Project/
    └── Events/                  ← 프로젝트별 이벤트 정의
        ├── GameEvents.cs
        ├── PlayerEvents.cs
        └── EnemyEvents.cs
```

**이벤트 struct 작성 예시**

```csharp
// Project/Events/PlayerEvents.cs
namespace Project.Events
{
    public struct PlayerDeadEvent { }

    public struct PlayerDamagedEvent
    {
        public int Amount;
    }

    public struct PlayerScoreChangedEvent
    {
        public int Score;
    }
}
```

---

## 의존성

없음

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `EventBus` | 정적 클래스. `Subscribe` / `Unsubscribe` / `Publish` 제공 |

### 주요 메서드

| 메서드 | 설명 |
|--------|------|
| `Subscribe<T>(Action<T>)` | 이벤트 구독 |
| `Unsubscribe<T>(Action<T>)` | 이벤트 구독 해제 |
| `Publish<T>(T)` | 이벤트 발행. 구독 중인 모든 핸들러에 전달 |

---

## 사용법

```csharp
// 구독 — 보통 OnEnable 또는 Start에서
EventBus.Subscribe<PlayerDeadEvent>(OnPlayerDead);

// 구독 해제 — 반드시 OnDestroy 또는 OnDisable에서 호출
EventBus.Unsubscribe<PlayerDeadEvent>(OnPlayerDead);

// 발행
EventBus.Publish(new PlayerDeadEvent());

// 데이터가 있는 이벤트 발행
EventBus.Publish(new PlayerDamagedEvent { Amount = 30 });

// 핸들러 정의
private void OnPlayerDead(PlayerDeadEvent e)
{
    // 처리 로직
}
```

---

## 예시 (심화)

```csharp
// 플레이어 체력 관리 — 이벤트 발행
public class PlayerHealth : MonoBehaviour
{
    private int _hp = 100;

    public void TakeDamage(int amount)
    {
        _hp -= amount;
        EventBus.Publish(new PlayerDamagedEvent { Amount = amount });

        if (_hp <= 0)
            EventBus.Publish(new PlayerDeadEvent());
    }
}

// HUD — 이벤트 구독
public class HUDManager : MonoBehaviour
{
    private void OnEnable()
    {
        EventBus.Subscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        EventBus.Subscribe<PlayerDeadEvent>(OnPlayerDead);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<PlayerDamagedEvent>(OnPlayerDamaged);
        EventBus.Unsubscribe<PlayerDeadEvent>(OnPlayerDead);
    }

    private void OnPlayerDamaged(PlayerDamagedEvent e)
    {
        UpdateHPBar(e.Amount);
    }

    private void OnPlayerDead(PlayerDeadEvent e)
    {
        ShowGameOverScreen();
    }
}
```

---

## 주의사항

- 이벤트는 `struct` 사용을 권장합니다. `class` 이벤트는 자주 발행될 경우 GC 부하를 유발할 수 있습니다.
- `Subscribe`한 쪽에서 반드시 `Unsubscribe`를 호출해야 합니다. 해제하지 않으면 오브젝트가 파괴된 후에도 콜백이 호출되어 오류가 발생할 수 있습니다.
- `Unsubscribe`는 `OnDestroy` 또는 `OnDisable`에서 호출하는 것을 권장합니다.
- 에디터에서는 Subscribe / Unsubscribe / Publish 시 콘솔에 로그가 출력됩니다. 빌드에서는 출력되지 않습니다.
- EventBus는 남발하면 이벤트 흐름 파악이 어려워집니다. 씬을 넘나드는 게임 로직 이벤트에만 사용하고, UI 인터랙션이나 로컬 이벤트는 UnityEvent 또는 직접 콜백을 사용하세요.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-03-25 | 최초 작성 |
