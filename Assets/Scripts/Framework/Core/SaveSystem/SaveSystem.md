# SaveSystem

JSON 파일과 PlayerPrefs 두 가지 방식의 데이터 저장을 지원하는 정적 유틸리티입니다.

---

## 개요

게임 데이터 저장 방식을 두 가지로 분리하여 제공합니다.

| 방식 | 용도 |
|------|------|
| **JSON** | 캐릭터, 인벤토리 등 복잡한 구조의 게임 데이터 |
| **PlayerPrefs** | 볼륨, 해상도 등 단순 설정값 |

> **서버 저장 전환 예정**
> 현재 JSON 데이터는 로컬 파일(`Application.persistentDataPath`)에 저장됩니다.
> 추후 서버 저장 방식으로 전환 시 `IStorageProvider` 인터페이스를 도입하여
> `LocalStorageProvider` / `RemoteStorageProvider` 로 교체하세요.

---

## 의존성

- `Newtonsoft.Json` (com.unity.nuget.newtonsoft-json) — JSON 직렬화/역직렬화

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `SaveSystem` | 저장/불러오기 정적 유틸리티 |

### JSON 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Save<T>(string key, T data)` | `void` | 데이터를 JSON 파일로 저장 |
| `Load<T>(string key, T defaultValue)` | `T` | JSON 파일에서 데이터 불러오기 |
| `Delete(string key)` | `void` | JSON 파일 삭제 |
| `Exists(string key)` | `bool` | JSON 파일 존재 여부 확인 |

### PlayerPrefs 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `SavePref<T>(string key, T value)` | `void` | PlayerPrefs에 값 저장 |
| `LoadPref<T>(string key, T defaultValue)` | `T` | PlayerPrefs에서 값 불러오기 |
| `DeletePref(string key)` | `void` | PlayerPrefs 키 삭제 |
| `ExistsPref(string key)` | `bool` | PlayerPrefs 키 존재 여부 확인 |

PlayerPrefs 지원 타입: `int`, `float`, `string`, `bool`

---

## 사용법

### JSON 저장/불러오기

```csharp
[System.Serializable]
public class PlayerData
{
    public string name;
    public int level;
    public float hp;
}

// 저장
var data = new PlayerData { name = "Hero", level = 5, hp = 100f };
SaveSystem.Save("playerData", data);

// 불러오기
PlayerData loaded = SaveSystem.Load<PlayerData>("playerData");

// 존재 여부 확인
if (SaveSystem.Exists("playerData"))
{
    // ...
}

// 삭제
SaveSystem.Delete("playerData");
```

### PlayerPrefs 저장/불러오기

```csharp
// 저장
SaveSystem.SavePref("masterVolume", 0.8f);
SaveSystem.SavePref("isFullscreen", true);
SaveSystem.SavePref("playerName", "Hero");

// 불러오기 (두 번째 인자는 키가 없을 때 반환할 기본값)
float volume = SaveSystem.LoadPref("masterVolume", 1.0f);
bool fullscreen = SaveSystem.LoadPref("isFullscreen", false);
string name = SaveSystem.LoadPref("playerName", "Unknown");

// 삭제
SaveSystem.DeletePref("masterVolume");
```

---

## 예시 (심화)

### 설정 매니저와 함께 사용

```csharp
public class SettingsManager : MonoBehaviour
{
    private const string KeyVolume = "masterVolume";
    private const string KeyFullscreen = "isFullscreen";

    public float MasterVolume { get; private set; }
    public bool IsFullscreen { get; private set; }

    private void Start()
    {
        MasterVolume = SaveSystem.LoadPref(KeyVolume, 1.0f);
        IsFullscreen = SaveSystem.LoadPref(KeyFullscreen, false);
        Apply();
    }

    public void SetVolume(float value)
    {
        MasterVolume = value;
        SaveSystem.SavePref(KeyVolume, value);
        Apply();
    }

    public void SetFullscreen(bool value)
    {
        IsFullscreen = value;
        SaveSystem.SavePref(KeyFullscreen, value);
        Apply();
    }

    private void Apply()
    {
        AudioListener.volume = MasterVolume;
        Screen.fullScreen = IsFullscreen;
    }
}
```

---

## 주의사항

- `Save<T>` 는 `Newtonsoft.Json`을 사용하므로 `[System.Serializable]` 어트리뷰트 없이도 직렬화됩니다.
- `Dictionary`, `private` 필드도 직렬화됩니다. `private` 필드를 제외하려면 `[JsonIgnore]` 어트리뷰트를 사용하세요.
- `SavePref`는 `int`, `float`, `string`, `bool` 타입만 지원합니다. 그 외 타입은 에러 로그를 출력하고 저장하지 않습니다.
- JSON 저장 경로(`Application.persistentDataPath`)는 플랫폼마다 다릅니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-01 | 최초 작성 |
