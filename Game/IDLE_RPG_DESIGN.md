# 방치형 RPG — 게임 기획서

Unity Generic Framework 샘플 프로젝트입니다.
프레임워크의 주요 모듈 연동을 검증하는 것이 목적입니다.

---

## 게임 개요

| 항목 | 내용 |
|------|------|
| 장르 | 방치형 RPG |
| 레퍼런스 | 리니지M, 뮤 오리진 |
| 화면 방향 | 세로 (Portrait) |
| 시점 | 3D 쿼터뷰 (카메라 고정 등각 투영) |
| 플랫폼 | 모바일 (Android / iOS) |
| 핵심 루프 | 필드 자동 사냥 → 골드/경험치 획득 → 강화/레벨업 → 더 강한 필드 진입 |

### 게임 특징

- 캐릭터가 필드에서 몬스터를 자동으로 타겟, 이동, 공격
- 자동 스킬 사용 (쿨타임마다 자동 발동)
- 앱을 끄고 있어도 오프라인 보상 지급 (방치 보상)
- 스테이지 / 필드 개념 — 구역마다 몬스터 종류와 난이도 상이
- 성장 콘텐츠 — 레벨업, 장비 강화, 스킬 강화

---

## 핵심 루프

```
[자동 전투]
  몬스터 스폰 → 캐릭터 자동 공격 → 몬스터 사망 → 골드/경험치 드롭
      ↓
[성장]
  레벨업 → 스탯 증가
  골드 → 장비 강화
      ↓
[스테이지 진행]
  현재 스테이지 클리어 → 다음 스테이지 이동
```

---

## 씬 구성

| 씬 | 설명 |
|----|------|
| `BootScene` | 초기 데이터 로드, 씬 전환 |
| `LobbyScene` | 캐릭터 정보, 강화, 설정 |
| `BattleScene` | 자동 전투 |

---

## 캐릭터

### 기본 스탯

| 스탯 | 설명 |
|------|------|
| HP | 최대 체력 |
| ATK | 공격력 |
| DEF | 방어력 |
| ATK_SPD | 공격 속도 |

### 레벨업

- 경험치 획득 → 레벨업 → 스탯 증가
- 레벨업 시 ToastManager로 알림

### 장비

- 무기 / 방어구 2슬롯
- 골드로 강화 (+1 ~ +10)
- 강화 수치에 따라 스탯 증가

---

## 전투

### 자동 전투 흐름

```
BattleScene 입장
  → 몬스터 ObjectPool에서 스폰
  → 캐릭터 StateMachine: Idle → Combat
  → 범위 내 몬스터 자동 타겟
  → 공격 쿨타임마다 자동 공격
  → 몬스터 사망 → 골드/경험치 드롭 → 다음 몬스터 스폰
  → 스테이지 몬스터 수 소진 → 스테이지 클리어
```

### 캐릭터 StateMachine

| 상태 | 설명 |
|------|------|
| `IdleState` | 주변에 몬스터 없음. 대기 |
| `CombatState` | 몬스터 타겟. 범위 내 접근 후 공격 |
| `DeadState` | HP 0. 일정 시간 후 부활 |

### 몬스터 StateMachine

| 상태 | 설명 |
|------|------|
| `IdleState` | 스폰 직후 대기 |
| `ChaseState` | 캐릭터를 향해 이동 |
| `AttackState` | 사거리 내 공격 |
| `DeadState` | 사망 연출 후 풀 반환 |

---

## UI 구성

### BattleScene HUD

| UI | 모듈 | 설명 |
|----|------|------|
| HP바 | UIManager | 캐릭터 HP 표시 |
| 스테이지 정보 | UIManager | 현재 스테이지, 몬스터 수 |
| 골드/경험치 | UIManager | 실시간 표시 |
| 레벨업 알림 | ToastManager | 레벨업 시 잠깐 표시 |
| 일시정지 | CommonPopupManager | 설정/나가기 |

### LobbyScene

| UI | 모듈 | 설명 |
|----|------|------|
| 캐릭터 정보 패널 | UIManager | 스탯, 장비 확인 |
| 강화 패널 | UIManager | 장비 강화 |
| 강화 확인 팝업 | CommonPopupManager | 골드 소모 확인 |
| 강화 결과 알림 | ToastManager | 성공/실패 알림 |
| 설정 패널 | UIManager | 언어, 음량 설정 |

---

## 데이터

### JSON 데이터 (InGameDataManager)

| 파일 | 키 | 설명 |
|------|----|------|
| `StageData.json` | stage_id | 스테이지 정보 (몬스터 종류, 수, 추천 전투력) |
| `MonsterData.json` | monster_id | 몬스터 스탯 |
| `ItemData.json` | item_id | 장비 기본 스탯 |
| `LevelData.json` | level | 레벨별 필요 경험치, 스탯 증가량 |

### 저장 데이터 (SaveSystem)

| 키 | 내용 |
|----|------|
| `player_level` | 현재 레벨 |
| `player_exp` | 현재 경험치 |
| `player_gold` | 보유 골드 |
| `equipment` | 장비 강화 수치 |
| `current_stage` | 현재 스테이지 |

---

## 활용 모듈 목록

| 모듈 | 사용처 |
|------|--------|
| `StateMachine` | 캐릭터 / 몬스터 상태 관리 |
| `ObjectPool` | 몬스터, 공격 이펙트, 드롭 아이템 |
| `EventBus` | 레벨업, 스테이지 클리어, 몬스터 사망 이벤트 |
| `AudioManager` | BGM(로비/전투), SFX(공격/레벨업/강화) |
| `SaveSystem` | 플레이어 데이터 저장/로드 |
| `InGameDataManager` | 스테이지/몬스터/아이템/레벨 데이터 로드 |
| `SceneLoader` | Boot → Lobby → Battle 씬 전환 |
| `InputManager` | 모바일 터치 입력 |
| `UIManager` | HUD, 로비 패널 관리 |
| `CommonPopupManager` | 강화 확인, 일시정지 팝업 |
| `ToastManager` | 레벨업, 강화 결과 알림 |
| `SafeAreaFitter` | 노치/홈 인디케이터 회피 |
| `LocalizationSystem` | 한국어/영어 지원 |

---

## 디렉토리 구조 (예정)

```
Assets/
├── Scripts/
│   ├── Framework/       ← 기존 프레임워크
│   └── Game/
│       ├── Boot/        ← BootScene 초기화
│       ├── Lobby/       ← 로비 로직
│       ├── Battle/      ← 전투 로직
│       │   ├── Character/
│       │   ├── Monster/
│       │   └── Stage/
│       ├── Data/        ← 데이터 클래스 (StageData, MonsterData 등)
│       └── UI/          ← 게임 전용 UI 패널
├── Prefabs/
│   ├── Characters/
│   ├── Monsters/
│   └── UI/
├── Resources/
│   ├── Data/            ← JSON 데이터
│   ├── Localization/    ← 다국어 JSON
│   └── UI/              ← UI 프리팹
└── Scenes/
    ├── BootScene
    ├── LobbyScene
    └── BattleScene
```

---

## 개발 순서 (예정)

1. BootScene — 데이터 로드, 씬 전환
2. 데이터 클래스 및 JSON 작성
3. BattleScene — 캐릭터, 몬스터, 전투 로직
4. LobbyScene — 강화, 캐릭터 정보
5. UI 연동
6. 저장/로드
7. 오디오
8. 다국어
