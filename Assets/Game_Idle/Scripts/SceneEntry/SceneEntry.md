# Scene Entry

각 씬의 진입점 스크립트입니다.
씬에 빈 GameObject를 만들고 해당 Entry 컴포넌트를 추가하세요.

## 씬 구성

| 씬 | Entry | 상태 |
|----|-------|------|
| BootScene | `BootEntry` | ✅ 완료 |
| MainScene | `MainEntry` | 🚧 진행 중 |
| LoginScene | — | ⬜ 추후 (서버 연결 시 추가) |

### MainScene 구조

자동사냥, 강화, 장비, 설정 등 **모든 컨텐츠가 MainScene 하나**에서 동작합니다.
UIManager 패널 오버레이 방식으로 씬 전환 없이 모든 기능에 접근합니다.

추후 던전/보스타임 같이 별도 연출이 필요한 컨텐츠는 독립 씬으로 분리합니다:
```
MainScene
    ↓ (추후)
DungeonScene / BossScene → MainScene 복귀
```

---

## 설계 원칙

- 각 Entry는 `MonoBehaviour`를 직접 상속합니다.
- `SceneController` 같은 베이스 클래스는 추가하지 않습니다.
  씬 수가 적고 공통 로직이 없으므로 **오버엔지니어링**이 됩니다.
- Entry는 씬 내 시스템을 초기화하고 연결하는 역할만 합니다.
  로직은 각 시스템(StageManager, PlayerController 등)에 위임합니다.

---

## 작업 목록

### BootEntry
- [x] 게임 데이터 로드 (`InGameDataManager.LoadAll`)
- [x] MainScene 전환 (`SceneLoader`)

### MainEntry
- [ ] 오프라인 보상 계산 및 팝업 (앱 종료 시각 기준)
- [ ] StageManager 초기화 (챕터 ID 전달)
- [ ] PlayerController 초기화

### LoginScene (추후)
- [ ] 서버 인증 연동 시 BootEntry → LoginEntry → MainEntry 순서로 추가
- [ ] BootEntry의 `_nextScene`을 `"LoginScene"`으로 변경

---

## 씬 전환 흐름

```
현재
BootScene → MainScene

추후 (서버 연동 시)
BootScene → LoginScene → MainScene
```
