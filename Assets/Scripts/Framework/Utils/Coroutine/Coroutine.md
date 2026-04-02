# Coroutine Utils

코루틴 기반의 타이머, 반복 실행, GC 최적화 유틸리티 모음입니다.

---

## 개요

| 클래스 | 설명 |
|--------|------|
| `WaitCache` | `WaitForSeconds` 인스턴스 캐싱으로 GC 압력 감소 |
| `CoroutineRunner` | MonoBehaviour 없이 코루틴을 실행할 수 있는 싱글톤 러너 |
| `CoroutineTimer` | Delay, Repeat, Lerp 등 코루틴 기반 타이머 유틸리티 |

세 클래스는 함께 동작합니다. `CoroutineTimer`는 내부적으로 `CoroutineRunner`와 `WaitCache`를 사용합니다.

---

## 의존성

- `Framework.Core.Singleton` — `CoroutineRunner`가 `PersistentMonoSingleton<T>` 상속

---

## API 레퍼런스

### WaitCache

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Get(float seconds)` | `WaitForSeconds` | 캐싱된 WaitForSeconds 반환 |
| `GetRealtime(float seconds)` | `WaitForSecondsRealtime` | 캐싱된 WaitForSecondsRealtime 반환 |
| `Clear()` | `void` | 캐시 전체 비우기 |

### CoroutineRunner

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `StartCoroutine(IEnumerator)` | `Coroutine` | 코루틴 실행 |
| `StopCoroutine(Coroutine)` | `void` | 특정 코루틴 중지 |
| `StopAllCoroutines()` | `void` | 모든 코루틴 중지 |

### CoroutineTimer

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Delay(float delay, Action callback)` | `Coroutine` | 지정 시간 후 콜백 실행 |
| `DelayRealtime(float delay, Action callback)` | `Coroutine` | TimeScale 무시, 지정 시간 후 콜백 실행 |
| `Repeat(float interval, Action callback, int count)` | `Coroutine` | 일정 간격 반복 실행. count=-1이면 무한 반복 |
| `WaitUntil(Func<bool> condition, Action callback)` | `Coroutine` | 조건 충족 시 콜백 실행 |
| `Lerp(float from, float to, float duration, Action<float> onUpdate, Action onComplete)` | `Coroutine` | 값 선형 보간하며 매 프레임 onUpdate 호출 |
| `Cancel(Coroutine coroutine)` | `void` | 실행 중인 타이머 중지 |

---

## 사용법

### 지연 실행

```csharp
// 2초 후 실행
CoroutineTimer.Delay(2f, () => Debug.Log("2초 경과"));

// TimeScale 무시 (일시정지 중에도 동작)
CoroutineTimer.DelayRealtime(2f, () => Debug.Log("실제 2초 경과"));
```

### 반복 실행

```csharp
// 1초마다 무한 반복
Coroutine handle = CoroutineTimer.Repeat(1f, () => Debug.Log("틱"));

// 3회만 반복
CoroutineTimer.Repeat(0.5f, () => Debug.Log("3회 반복"), count: 3);

// 중지
CoroutineTimer.Cancel(handle);
```

### 조건 대기

```csharp
// _isReady가 true가 될 때까지 대기 후 실행
CoroutineTimer.WaitUntil(() => _isReady, () => Debug.Log("준비 완료"));
```

### 값 보간 (Lerp)

```csharp
// 0에서 1까지 2초간 보간하며 UI 알파 적용
CoroutineTimer.Lerp(0f, 1f, 2f,
    onUpdate: value => _canvasGroup.alpha = value,
    onComplete: () => Debug.Log("페이드 인 완료")
);
```

### MonoBehaviour 없는 일반 클래스에서 코루틴 실행

```csharp
public class GameLogic
{
    public void StartLoading()
    {
        CoroutineRunner.Instance.StartCoroutine(LoadRoutine());
    }

    private IEnumerator LoadRoutine()
    {
        yield return WaitCache.Get(1f);
        Debug.Log("로딩 완료");
    }
}
```

---

## 예시 (심화)

### 쿨타임 시스템

```csharp
public class AbilitySystem : MonoBehaviour
{
    private bool _isCooldown;
    private Coroutine _cooldownHandle;

    public void UseAbility()
    {
        if (_isCooldown)
        {
            return;
        }

        _isCooldown = true;
        Debug.Log("스킬 사용!");

        _cooldownHandle = CoroutineTimer.Delay(3f, () =>
        {
            _isCooldown = false;
            Debug.Log("쿨타임 완료");
        });
    }

    private void OnDestroy()
    {
        CoroutineTimer.Cancel(_cooldownHandle);
    }
}
```

### 체력 회복 타이머

```csharp
private Coroutine _regenHandle;

public void StartRegen(float interval, int amount)
{
    StopRegen();
    _regenHandle = CoroutineTimer.Repeat(interval, () =>
    {
        _hp = Mathf.Min(_hp + amount, _maxHp);
    });
}

public void StopRegen()
{
    CoroutineTimer.Cancel(_regenHandle);
}
```

---

## 주의사항

- `CoroutineRunner`는 `PersistentMonoSingleton`이므로 씬 전환 후에도 유지됩니다. 씬에 별도로 배치할 필요 없이 첫 호출 시 자동 생성됩니다.
- `WaitCache`는 한 번 캐싱된 인스턴스를 재사용하므로, 동일한 `float` 값에 대해 항상 같은 객체를 반환합니다. `WaitForSecondsRealtime`은 내부 상태를 갖지 않아 재사용이 안전합니다.
- `CoroutineTimer.Cancel`에 `null`을 전달하면 아무 동작도 하지 않습니다.
- `Lerp`는 `Time.deltaTime` 기반이므로 TimeScale의 영향을 받습니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-03 | 최초 작성 (WaitCache, CoroutineRunner, CoroutineTimer) |
