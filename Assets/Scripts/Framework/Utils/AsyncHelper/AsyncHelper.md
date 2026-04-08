# AsyncHelper

UniTask 기반의 비동기 유틸리티 모음입니다.
async/await 패턴을 간결하게 사용할 수 있도록 래핑합니다.

---

## 개요

Unity에서 비동기 처리 시 `Task`는 메인 스레드와 충돌 위험이 있고 GC 부담이 있습니다.
`UniTask`는 Unity에 최적화된 zero-allocation 비동기 솔루션이며, `AsyncHelper`는 이를 더 간결하게 사용할 수 있도록 래핑합니다.

---

## 의존성

- `com.cysharp.unitask` — UniTask 패키지

---

## API 레퍼런스

### 대기

| 메서드 | 설명 |
|--------|------|
| `Delay(float seconds, CancellationToken)` | 지정한 시간(초)만큼 대기 |
| `DelayFrame(int frameCount, CancellationToken)` | 지정한 프레임 수만큼 대기 |
| `NextFrame(CancellationToken)` | 다음 프레임까지 대기 |
| `WaitForFixedUpdate(CancellationToken)` | 다음 FixedUpdate까지 대기 |
| `WaitForEndOfFrame(CancellationToken)` | 다음 EndOfFrame까지 대기 |

### 조건 대기

| 메서드 | 설명 |
|--------|------|
| `WaitUntil(Func<bool>, CancellationToken)` | 조건이 true가 될 때까지 대기 |
| `WaitWhile(Func<bool>, CancellationToken)` | 조건이 false가 될 때까지 대기 |

### 취소

| 메서드 | 설명 |
|--------|------|
| `CreateCTS()` | 새로운 CancellationTokenSource 생성 |
| `GetCancellationToken(MonoBehaviour)` | GameObject 수명과 연동된 CancellationToken 반환 |

---

## 사용법

### 지연 실행

```csharp
private async UniTaskVoid SpawnAfterDelay()
{
    await AsyncHelper.Delay(2f);
    Spawn();
}
```

### GameObject 수명 연동 취소

```csharp
private async UniTaskVoid StartTimer()
{
    var token = AsyncHelper.GetCancellationToken(this);

    await AsyncHelper.Delay(5f, token);
    OnTimerEnd(); // GameObject 파괴 시 자동 취소, 호출되지 않음
}
```

### 수동 취소

```csharp
private CancellationTokenSource _cts;

private async UniTaskVoid StartCountdown()
{
    _cts = AsyncHelper.CreateCTS();
    await AsyncHelper.Delay(10f, _cts.Token);
    OnCountdownEnd();
}

private void Cancel()
{
    _cts?.Cancel();
    _cts?.Dispose();
    _cts = null;
}
```

### 조건 대기

```csharp
private async UniTaskVoid WaitForReady()
{
    await AsyncHelper.WaitUntil(() => _isReady);
    StartGame();
}
```

### 다음 프레임 대기

```csharp
private async UniTaskVoid Initialize()
{
    // UI가 완전히 초기화된 후 다음 프레임에 실행
    await AsyncHelper.NextFrame();
    RefreshLayout();
}
```

---

## 주의사항

- `async UniTask` 메서드에서 예외가 발생하면 기본적으로 무시됩니다. 로그 확인이 필요하면 `try/catch`를 사용하세요.
- `GetCancellationToken(this)`를 사용하면 GameObject 파괴 시 자동으로 취소되어 메모리 누수를 방지합니다. 가능하면 수동 CTS보다 이 방식을 권장합니다.
- `CreateCTS()`로 생성한 CTS는 사용 후 반드시 `Dispose()`를 호출하세요.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-08 | 최초 작성 (Delay / Frame / Condition / Cancellation) |
