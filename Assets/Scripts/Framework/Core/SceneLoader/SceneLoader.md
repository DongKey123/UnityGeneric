# SceneLoader

비동기 씬 전환과 로딩 화면을 관리하는 싱글톤 매니저입니다.

---

## 개요

씬 전환 시 발생하는 로딩을 비동기로 처리하고, 진행률 이벤트와 커스텀 트랜지션 효과를 지원합니다.
`DontDestroyOnLoad`로 유지되므로 어느 씬에서든 동일한 인스턴스를 통해 씬 전환을 요청할 수 있습니다.

**지원 방식:**
- **직접 전환**: 현재 씬에서 목적지 씬으로 바로 로딩
- **로딩씬 경유**: 로딩 씬을 중간에 거쳐 진행률 UI를 표시한 후 목적지 씬으로 이동
- **트랜지션 효과**: `ISceneTransition` 구현체를 주입해 페이드, 슬라이드 등 전환 연출 적용

---

## 의존성

- `Framework.Core.Singleton` — `PersistentMonoSingleton<T>` 상속

---

## API 레퍼런스

### 클래스 / 인터페이스

| 이름 | 설명 |
|------|------|
| `SceneLoader` | 씬 전환 관리 싱글톤 (PersistentMonoSingleton) |
| `ISceneTransition` | 커스텀 전환 효과 인터페이스 |

### 프로퍼티

| 프로퍼티 | 타입 | 설명 |
|----------|------|------|
| `IsLoading` | `bool` | 씬 로딩 진행 중 여부 |

### 이벤트

| 이벤트 | 타입 | 설명 |
|--------|------|------|
| `OnLoadStart` | `Action` | 씬 로딩 시작 시 발생 |
| `OnLoadProgress` | `Action<float>` | 진행률 갱신 시 발생 (0~1) |
| `OnLoadComplete` | `Action` | 씬 로딩 완료 및 활성화 후 발생 |

### 주요 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `LoadScene(string sceneName, ISceneTransition transition)` | `void` | 씬 이름으로 비동기 전환 |
| `LoadScene(int sceneIndex, ISceneTransition transition)` | `void` | 씬 인덱스로 비동기 전환 |
| `LoadSceneWithLoadingScene(string targetScene, string loadingScene)` | `void` | 로딩 씬 경유 전환 |
| `ReloadScene(ISceneTransition transition)` | `void` | 현재 씬 재시작 |

### ISceneTransition

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `OnTransitionIn()` | `IEnumerator` | 로딩 시작 전 실행 (화면 가리기) |
| `OnTransitionOut()` | `IEnumerator` | 로딩 완료 후 실행 (화면 열기) |

---

## 사용법

### 기본 씬 전환

```csharp
// 씬 이름으로 전환
SceneLoader.Instance.LoadScene("Stage01");

// 씬 인덱스로 전환
SceneLoader.Instance.LoadScene(1);

// 현재 씬 재시작
SceneLoader.Instance.ReloadScene();
```

### 로딩 씬 경유 전환

```csharp
// LoadingScene을 중간에 거쳐 Stage01으로 이동
SceneLoader.Instance.LoadSceneWithLoadingScene("Stage01", "LoadingScene");
```

로딩 씬의 UI 컨트롤러에서 진행률을 구독합니다.

```csharp
public class LoadingUIController : MonoBehaviour
{
    [SerializeField] private Slider _progressBar;

    private void OnEnable()
    {
        SceneLoader.Instance.OnLoadProgress += UpdateProgress;
        SceneLoader.Instance.OnLoadComplete += OnComplete;
    }

    private void OnDisable()
    {
        SceneLoader.Instance.OnLoadProgress -= UpdateProgress;
        SceneLoader.Instance.OnLoadComplete -= OnComplete;
    }

    private void UpdateProgress(float progress)
    {
        _progressBar.value = progress;
    }

    private void OnComplete()
    {
        Debug.Log("씬 로딩 완료!");
    }
}
```

---

## 예시 (심화)

### 커스텀 페이드 트랜지션 구현

```csharp
using System.Collections;
using Framework.Core.SceneLoader;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour, ISceneTransition
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _duration = 0.5f;

    public IEnumerator OnTransitionIn()
    {
        // 화면을 서서히 가림 (알파 0 → 1)
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = Mathf.Clamp01(elapsed / _duration);
            yield return null;
        }
        _canvasGroup.alpha = 1f;
    }

    public IEnumerator OnTransitionOut()
    {
        // 화면을 서서히 열음 (알파 1 → 0)
        float elapsed = 0f;
        while (elapsed < _duration)
        {
            elapsed += Time.deltaTime;
            _canvasGroup.alpha = 1f - Mathf.Clamp01(elapsed / _duration);
            yield return null;
        }
        _canvasGroup.alpha = 0f;
    }
}
```

```csharp
// 페이드 트랜지션과 함께 씬 전환
FadeTransition fade = GetComponent<FadeTransition>();
SceneLoader.Instance.LoadScene("Stage02", fade);
```

---

## 주의사항

- `SceneLoader`는 `PersistentMonoSingleton`이므로 씬 전환 후에도 파괴되지 않습니다. 씬에 직접 배치하거나 코드에서 `SceneLoader.Instance`로 접근하면 자동 생성됩니다.
- 로딩 중 추가 `LoadScene` 호출은 무시됩니다. `IsLoading`을 확인 후 호출하세요.
- `OnLoadProgress` / `OnLoadComplete` 이벤트는 구독한 쪽에서 반드시 해제해야 합니다. 특히 로딩 씬의 오브젝트가 씬 전환 후 파괴되는 경우 `OnDisable` 또는 `OnDestroy`에서 해제하세요.
- `LoadSceneWithLoadingScene` 사용 시 로딩 씬에서 `OnLoadProgress`를 구독하는 시점이 한 프레임 지연될 수 있습니다. 초기 진행률 값이 0인 상태로 UI를 초기화해두는 것을 권장합니다.
- `LoadSceneAsync`의 `progress`는 최대 0.9까지만 올라가며, 씬이 완전히 로드된 후 `allowSceneActivation = true` 시점에 1.0이 됩니다. SceneLoader는 이를 0~1로 정규화하여 이벤트를 발생시킵니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-01 | 최초 작성 |
