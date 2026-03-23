# Singleton

Unity 프로젝트에서 사용하는 싱글톤 패턴 모음. 순수 C# 싱글톤과 MonoBehaviour 기반 싱글톤 3종을 제공합니다.

---

## 개요

싱글톤 패턴을 목적에 맞게 선택할 수 있도록 3가지 클래스를 제공합니다.

| 클래스 | MonoBehaviour | 씬 전환 유지 | 자동 생성 | 스레드 세이프 |
|--------|:---:|:---:|:---:|:---:|
| `Singleton<T>` | ❌ | — | ✅ | ✅ |
| `MonoSingleton<T>` | ✅ | ❌ | ✅ | ❌ |
| `PersistentMonoSingleton<T>` | ✅ | ✅ | ❌ | ❌ |

---

## 각 클래스 상세 설명

### Singleton\<T\> — 순수 C# 싱글톤

MonoBehaviour를 상속하지 않는 순수 C# 싱글톤입니다.
Unity 오브젝트 라이프사이클(Awake, Update 등)과 완전히 분리되어 동작하며, lock을 사용해 스레드 세이프하게 구현되어 있습니다.

**이런 경우에 사용하세요**
- `Update`, `Coroutine`, `GameObject` 등 Unity 기능이 전혀 필요 없는 경우
- 설정값, 데이터 저장소, 계산 로직 등 순수 데이터/로직 관리 클래스
- 멀티스레드 환경(비동기 로딩, 네트워크 등)에서 안전하게 접근해야 하는 경우

**이런 경우에는 사용하지 마세요**
- `StartCoroutine`, `Invoke`, `MonoBehaviour` 이벤트 함수가 필요한 경우
- `Transform`, `GameObject`를 직접 다뤄야 하는 경우

<details>
<summary>기본 개념 — 스레드 세이프 싱글톤이란?</summary>

멀티스레드 환경에서는 두 스레드가 동시에 `Instance`에 접근하면 인스턴스가 두 개 생성될 수 있습니다.
이를 방지하기 위해 `lock`을 사용해 한 번에 하나의 스레드만 인스턴스 생성 구간에 진입하도록 보장합니다.

```csharp
lock (_lock)
{
    if (_instance == null)
        _instance = new T(); // 이 구간은 동시에 하나의 스레드만 실행
}
```

Unity의 메인 스레드만 사용하는 일반 게임 로직에서는 큰 의미가 없지만,
`Task`, `Thread`, `async/await` 등 비동기 처리가 포함된 경우 안전하게 사용할 수 있습니다.

</details>

---

### MonoSingleton\<T\> — 씬 한정 MonoBehaviour 싱글톤

현재 씬 안에서만 살아있는 MonoBehaviour 싱글톤입니다.
씬에 배치되어 있지 않아도 `Instance` 접근 시 자동으로 `GameObject`를 생성합니다.
씬이 전환되면 오브젝트가 파괴되고 인스턴스도 초기화됩니다.

**이런 경우에 사용하세요**
- 씬마다 독립적으로 동작해야 하는 매니저 (씬 전환 시 리셋이 필요한 경우)
- `Coroutine`, `Update`, `MonoBehaviour` 이벤트 함수가 필요하지만 씬 간 유지는 불필요한 경우
- UI 관리, 씬 내 이벤트 처리 등 씬 로컬 로직

**이런 경우에는 사용하지 마세요**
- 씬 전환 후에도 상태를 유지해야 하는 경우 → `PersistentMonoSingleton` 사용

<details>
<summary>기본 개념 — MonoBehaviour 싱글톤이란?</summary>

Unity의 `MonoBehaviour`는 `new` 키워드로 직접 생성할 수 없고, 반드시 `GameObject`에 컴포넌트로 붙어야 존재할 수 있습니다.
MonoBehaviour 싱글톤은 이 제약 안에서 "인스턴스가 하나만 존재하도록 보장"하는 패턴입니다.

`Awake`에서 이미 인스턴스가 존재하면 자신을 `Destroy`해 중복을 제거하는 방식으로 동작합니다.

```csharp
protected virtual void Awake()
{
    if (_instance == null)
    {
        _instance = this as T;
        OnInitialize();
    }
    else
    {
        Destroy(gameObject); // 중복 오브젝트 제거
    }
}
```

`MonoSingleton`은 여기에 더해, 씬에 오브젝트가 없을 때 `AddComponent`로 자동 생성하는 기능을 포함합니다.

</details>

---

### PersistentMonoSingleton\<T\> — 씬 유지 MonoBehaviour 싱글톤

`DontDestroyOnLoad`가 적용된 MonoBehaviour 싱글톤입니다.
씬 전환 후에도 파괴되지 않고 게임 전체에서 유지됩니다.
씬에 직접 배치해야만 동작하며, 씬에 없을 경우 `Instance`가 null을 반환합니다.

**이런 경우에 사용하세요**
- Inspector에서 참조나 설정값을 직접 연결해야 하는 경우
- SoundManager, SceneLoader처럼 앱 생명주기 동안 유지되어야 하는 시스템
- 씬 배치가 명시적으로 이루어져야 하는 경우 (의도적인 제약)

**이런 경우에는 사용하지 마세요**
- 씬 전환 시 리셋이 필요한 경우 → `MonoSingleton` 사용
- Unity 기능이 필요 없는 경우 → `Singleton<T>` 사용

<details>
<summary>기본 개념 — DontDestroyOnLoad란?</summary>

Unity는 씬이 전환될 때 기존 씬의 모든 `GameObject`를 파괴합니다.
`DontDestroyOnLoad(gameObject)`를 호출하면 해당 오브젝트를 씬 전환에서 제외시켜 유지할 수 있습니다.

```csharp
protected virtual void Awake()
{
    if (_instance == null)
    {
        _instance = this as T;
        DontDestroyOnLoad(gameObject); // 씬 전환 후에도 유지
        OnInitialize();
    }
    else
    {
        Destroy(gameObject);
    }
}
```

반드시 씬에 직접 배치된 오브젝트에서만 동작합니다.
씬에 배치하지 않으면 인스턴스 자체가 존재하지 않으므로 `Instance`가 null을 반환합니다.

</details>

---

## 의존성

없음

---

## API 레퍼런스

### 클래스 / 인터페이스

| 이름 | 설명 |
|------|------|
| `Singleton<T>` | 순수 C# 싱글톤. MonoBehaviour 없이 사용. 스레드 세이프. `Release()`로 수동 해제 가능. |
| `MonoSingleton<T>` | 씬 한정 MonoBehaviour 싱글톤. 씬에 없으면 자동 생성. 씬 전환 시 파괴됨. |
| `PersistentMonoSingleton<T>` | 씬에 직접 배치하는 MonoBehaviour 싱글톤. `DontDestroyOnLoad`로 씬 전환 후에도 유지. |

### 주요 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Instance` (프로퍼티) | `T` | 싱글톤 인스턴스 반환 |
| `Release()` | `void` | 인스턴스 해제 (`Singleton<T>` 전용) |
| `OnInitialize()` | `void` | 초기화 시 호출되는 오버라이드 가능 메서드 |

---

## 사용법

```csharp
// 순수 C# 싱글톤 — Unity 기능 불필요한 설정/데이터 클래스
public class GameSettings : Singleton<GameSettings>
{
    public int Volume { get; set; } = 100;

    protected override void OnInitialize()
    {
        Volume = PlayerPrefs.GetInt("Volume", 100);
    }
}

GameSettings.Instance.Volume = 80;
GameSettings.Release(); // 인스턴스 해제 (다음 접근 시 재생성)
```

```csharp
// 씬 한정 MonoBehaviour 싱글톤 (씬 전환 시 파괴, 씬에 없으면 자동 생성)
public class UIManager : MonoSingleton<UIManager>
{
    protected override void OnInitialize()
    {
        // Awake 대신 여기서 초기화
    }
}

UIManager.Instance.ShowHUD(); // 씬에 없어도 자동으로 GameObject 생성
```

```csharp
// 씬 유지 MonoBehaviour 싱글톤 (씬에 직접 배치 필수)
public class SoundManager : PersistentMonoSingleton<SoundManager>
{
    [SerializeField] private AudioSource _bgmSource;

    protected override void OnInitialize()
    {
        _bgmSource.Play();
    }
}
```

---

## 예시 (심화)

```csharp
// 씬마다 다른 UI를 관리하는 매니저
// → 씬 전환 시 이전 UI가 제거되고 새 씬에서 새로 초기화되어야 하므로 MonoSingleton 사용
public class HUDManager : MonoSingleton<HUDManager>
{
    protected override void OnInitialize()
    {
        RefreshUI();
    }
}
```

---

## 주의사항

- `Singleton<T>` 는 `new()` 제약이 있으므로 반드시 `public` 기본 생성자가 필요합니다.
- `MonoSingleton<T>` 는 씬 전환 시 파괴되므로, 씬 간 유지가 필요한 경우 `PersistentMonoSingleton<T>`을 사용하세요.
- `PersistentMonoSingleton<T>` 는 반드시 씬에 직접 배치해야 합니다. 씬에 없으면 `Instance`가 null을 반환합니다.
- `PersistentMonoSingleton<T>` 는 씬 전환 후 유지되므로 중복 생성에 주의하세요.
- 모든 MonoBehaviour 싱글톤은 `Awake` 대신 `OnInitialize()`를 오버라이드하세요.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.1.0 | 2026-03-23 | LazySingleton 제거, 3종 체계로 정리 |
| 1.0.1 | 2026-03-17 | MonoSingleton 자동 생성 추가, 문서 설명 보강 |
| 1.0.0 | 2026-03-17 | 최초 작성 |
