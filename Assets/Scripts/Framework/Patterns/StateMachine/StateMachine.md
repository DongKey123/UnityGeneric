# StateMachine\<T\>

제네릭 유한 상태 머신(FSM)입니다.
객체가 여러 상태 중 하나에 있고, 조건에 따라 상태가 전환되는 패턴입니다.

---

## 개요

if/else로 상태를 분기하면 상태가 늘어날수록 코드가 복잡해집니다.
StateMachine은 각 상태를 독립된 클래스로 분리하여 상태별 로직을 캡슐화합니다.

- `StateMachine<T>` — 상태 전환 관리. Owner 타입을 제네릭으로 주입
- `IState<T>` — 상태 인터페이스 (Enter / Update / Exit)
- `BaseState<T>` — 필요한 메서드만 override할 수 있는 추상 기반 클래스

---

## 의존성

없음

---

## API 레퍼런스

### 클래스 / 인터페이스

| 이름 | 설명 |
|------|------|
| `StateMachine<T>` | 상태 전환 관리 |
| `IState<T>` | 상태 인터페이스 |
| `BaseState<T>` | 상태 추상 기반 클래스 |

### StateMachine\<T\> 이벤트

| 이벤트 | 타입 | 설명 |
|--------|------|------|
| `OnStateChanged` | `Action<IState<T>, IState<T>>` | 상태 전환 시 발생 (이전 상태, 새 상태) |

### StateMachine\<T\> 프로퍼티

| 프로퍼티 | 타입 | 설명 |
|----------|------|------|
| `CurrentState` | `IState<T>` | 현재 활성화된 상태 |

### StateMachine\<T\> 메서드

| 메서드 | 설명 |
|--------|------|
| `SetInitialState(IState<T>)` | 초기 상태 설정. Enter() 호출 |
| `Update()` | 현재 상태의 Update() 호출. 매 프레임 호출 필요 |
| `ChangeState(IState<T>)` | 상태 전환. 현재 상태 Exit() → 새 상태 Enter() |
| `IsState<TState>()` | 현재 상태가 TState인지 확인 |

### IState\<T\> 메서드

| 메서드 | 설명 |
|--------|------|
| `Enter(T owner)` | 상태 진입 시 한 번 호출 |
| `Update(T owner)` | 상태 활성화 중 매 프레임 호출 |
| `Exit(T owner)` | 상태 이탈 시 한 번 호출 |

---

## 사용법

### 상태 구현

```csharp
using Framework.Patterns.StateMachine;

public class PlayerIdleState : BaseState<PlayerController>
{
    public override void Enter(PlayerController owner)
    {
        owner.Animator.Play("Idle");
    }

    public override void Update(PlayerController owner)
    {
        if (owner.InputDirection != Vector2.zero)
        {
            owner.StateMachine.ChangeState(owner.RunState);
        }
    }
}

public class PlayerRunState : BaseState<PlayerController>
{
    public override void Enter(PlayerController owner)
    {
        owner.Animator.Play("Run");
    }

    public override void Update(PlayerController owner)
    {
        owner.Move(owner.InputDirection);

        if (owner.InputDirection == Vector2.zero)
        {
            owner.StateMachine.ChangeState(owner.IdleState);
        }
    }

    public override void Exit(PlayerController owner)
    {
        owner.StopMove();
    }
}
```

### StateMachine 초기화 및 업데이트

```csharp
public class PlayerController : MonoBehaviour
{
    public StateMachine<PlayerController> StateMachine { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerRunState RunState { get; private set; }

    private void Awake()
    {
        StateMachine = new StateMachine<PlayerController>(this);
        IdleState = new PlayerIdleState();
        RunState = new PlayerRunState();
    }

    private void Start()
    {
        StateMachine.SetInitialState(IdleState);
    }

    private void Update()
    {
        StateMachine.Update();
    }
}
```

### 상태 확인 및 이벤트 구독

```csharp
// 현재 상태 확인
if (StateMachine.IsState<PlayerIdleState>())
{
    // ...
}

// 상태 전환 이벤트
StateMachine.OnStateChanged += (prev, next) =>
{
    Debug.Log($"{prev?.GetType().Name} → {next.GetType().Name}");
};
```

---

## 실전 적용 가이드

### StateMachine이 적합한 경우

| 대상 | 상태 예시 |
|------|-----------|
| 플레이어 | Idle / Run / Jump / Attack / Dead |
| 단순 적 AI | Patrol / Chase / Attack |
| 게임 흐름 | Title / Loading / InGame / Pause / GameOver |
| NPC | Idle / Walk / Talk |
| 보스 패턴 | Phase1 / Phase2 / Enrage |

상태 수가 **10개 이하**이고, 전환 조건이 단순하면 StateMachine으로 충분합니다.

---

### StateMachine만으로 부족한 경우

**적 AI가 복잡해질 때** — 순찰하다가 시야에 들어오면 추격, 거리가 가까우면 근접 공격, 멀면 원거리 공격, 체력이 낮으면 도망, 동료 호출 등 조건이 중첩되면 StateMachine의 상태 수가 폭발합니다.

이 경우 **BehaviorTree** 도입을 검토하세요.

> 단, BehaviorTree 직접 구현은 복잡합니다. Unity **Behavior** 패키지(공식) 또는 **NodeCanvas**, **Behavior Designer** 같은 에셋 사용이 현실적입니다.

---

### 계층형 StateMachine (HFSM)

보스처럼 페이즈가 있고 각 페이즈 안에 세부 상태가 있는 경우, 상태 안에 StateMachine을 중첩할 수 있습니다.

```csharp
// 보스 Phase1 상태 내부에 세부 StateMachine
public class BossPhase1State : BaseState<BossController>
{
    private StateMachine<BossController> _innerFSM;

    public override void Enter(BossController owner)
    {
        _innerFSM = new StateMachine<BossController>(owner);
        _innerFSM.SetInitialState(new BossIdleState());
    }

    public override void Update(BossController owner)
    {
        _innerFSM.Update();
    }
}
```

> 단순한 구조에 HFSM을 도입하면 오버엔지니어링입니다. 일반 StateMachine으로 시작하고 복잡도가 늘어날 때 리팩토링하세요.

---

## 주의사항

- `SetInitialState()`를 호출하지 않으면 `Update()` 호출 시 아무 동작도 하지 않습니다.
- 동일한 상태로 `ChangeState()`를 호출하면 무시됩니다. (Enter/Exit 중복 호출 방지)
- 상태 클래스는 **stateless**로 유지하세요. 상태별 데이터는 Owner에 보관하고 상태는 로직만 담당합니다.
- `Update()` 내부에서 `ChangeState()`를 호출하면 그 프레임에 바로 전환됩니다. 전환 후 남은 코드가 실행되지 않도록 `return`을 습관화하세요.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-08 | 최초 작성 |
