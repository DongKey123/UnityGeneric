# AudioManager

BGM과 SFX를 통합 관리하는 싱글톤 오디오 매니저입니다.

---

## 개요

AudioManager는 게임에서 반복적으로 필요한 오디오 기능을 하나의 싱글톤으로 제공합니다.

| 기능 | 설명 |
|------|------|
| **BGM** | 단일 AudioSource로 배경음악 재생, 페이드 인/아웃 지원 |
| **SFX** | AudioSource 풀을 사용해 효과음 동시 재생 지원 |
| **볼륨/뮤트** | BGM/SFX 독립적으로 조절, PlayerPrefs 자동 저장/복원 |

`DontDestroyOnLoad`로 유지되므로 씬 전환 후에도 BGM이 끊기지 않습니다.

---

## 의존성

- `Framework.Core.Singleton` — `PersistentMonoSingleton<T>` 상속

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `AudioManager` | BGM/SFX 통합 관리 싱글톤 |

### 프로퍼티

| 프로퍼티 | 타입 | 설명 |
|----------|------|------|
| `BGMVolume` | `float` | 현재 BGM 볼륨 (0~1) |
| `SFXVolume` | `float` | 현재 SFX 볼륨 (0~1) |
| `IsBGMMuted` | `bool` | BGM 뮤트 상태 |
| `IsSFXMuted` | `bool` | SFX 뮤트 상태 |
| `IsBGMPlaying` | `bool` | BGM 재생 중 여부 |

### 주요 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `PlayBGM(AudioClip clip, bool fade)` | `void` | BGM 재생 (fade: 페이드 인 여부) |
| `StopBGM(bool fade)` | `void` | BGM 중지 (fade: 페이드 아웃 여부) |
| `PauseBGM()` | `void` | BGM 일시 정지 |
| `ResumeBGM()` | `void` | BGM 재개 |
| `PlaySFX(AudioClip clip, float volumeScale)` | `void` | SFX 재생 |
| `PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeScale)` | `void` | 지정 좌표에서 SFX 재생 |
| `StopAllSFX()` | `void` | 모든 SFX 즉시 중지 |
| `SetBGMVolume(float volume)` | `void` | BGM 볼륨 설정 및 저장 |
| `SetSFXVolume(float volume)` | `void` | SFX 볼륨 설정 및 저장 |
| `MuteBGM(bool mute)` | `void` | BGM 뮤트 설정 및 저장 |
| `MuteSFX(bool mute)` | `void` | SFX 뮤트 설정 및 저장 |

---

## 사용법

### 씬에 배치

Hierarchy에 빈 오브젝트를 만들고 `AudioManager` 컴포넌트를 추가합니다.
`PersistentMonoSingleton` 특성상 Boot 씬 또는 최초 씬에 한 번만 배치하면 됩니다.

| 인스펙터 필드 | 기본값 | 설명 |
|--------------|--------|------|
| SFX Pool Size | 8 | 초기 SFX AudioSource 풀 크기 |
| Fade Duration | 1.0 | 페이드 인/아웃 시간 (초) |

### BGM 재생

```csharp
// 일반 재생
AudioManager.Instance.PlayBGM(bgmClip);

// 페이드 인으로 재생
AudioManager.Instance.PlayBGM(bgmClip, fade: true);

// 페이드 아웃 후 중지
AudioManager.Instance.StopBGM(fade: true);
```

### SFX 재생

```csharp
// 기본 재생
AudioManager.Instance.PlaySFX(sfxClip);

// 볼륨 스케일 적용 (SFX 기본 볼륨 × 0.5)
AudioManager.Instance.PlaySFX(sfxClip, volumeScale: 0.5f);

// 3D 위치에서 재생
AudioManager.Instance.PlaySFXAtPosition(sfxClip, transform.position);
```

### 볼륨 조절

```csharp
// 볼륨 설정 (PlayerPrefs 자동 저장)
AudioManager.Instance.SetBGMVolume(0.8f);
AudioManager.Instance.SetSFXVolume(0.6f);

// 뮤트
AudioManager.Instance.MuteBGM(true);
AudioManager.Instance.MuteSFX(true);
```

---

## 예시 (심화)

### 씬 전환 시 BGM 교체

```csharp
public class GameSceneController : MonoBehaviour
{
    [SerializeField] private AudioClip _gameBGM;

    private void Start()
    {
        AudioManager.Instance.PlayBGM(_gameBGM, fade: true);
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopBGM(fade: true);
    }
}
```

### 설정 UI와 연동

```csharp
public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider _bgmSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Toggle _bgmMuteToggle;
    [SerializeField] private Toggle _sfxMuteToggle;

    private void Start()
    {
        _bgmSlider.value = AudioManager.Instance.BGMVolume;
        _sfxSlider.value = AudioManager.Instance.SFXVolume;
        _bgmMuteToggle.isOn = AudioManager.Instance.IsBGMMuted;
        _sfxMuteToggle.isOn = AudioManager.Instance.IsSFXMuted;

        _bgmSlider.onValueChanged.AddListener(AudioManager.Instance.SetBGMVolume);
        _sfxSlider.onValueChanged.AddListener(AudioManager.Instance.SetSFXVolume);
        _bgmMuteToggle.onValueChanged.AddListener(AudioManager.Instance.MuteBGM);
        _sfxMuteToggle.onValueChanged.AddListener(AudioManager.Instance.MuteSFX);
    }
}
```

---

## 주의사항

- `AudioManager`는 `PersistentMonoSingleton`이므로 씬 전환 후에도 파괴되지 않습니다. Boot 씬 등 최초 씬에 한 번만 배치하세요.
- SFX 풀이 부족하면 자동으로 AudioSource가 추가됩니다. 동시 재생이 많은 경우 `SFX Pool Size`를 충분히 설정하세요.
- `PlaySFXAtPosition`은 Unity 내장 `AudioSource.PlayClipAtPoint`를 사용하므로 3D 공간 감쇠가 적용됩니다.
- BGM 뮤트 상태에서 `PlayBGM`을 호출하면 클립은 교체되지만 소리는 나지 않습니다. `MuteBGM(false)` 호출 시 정상 볼륨으로 복원됩니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-03 | 최초 작성 |
