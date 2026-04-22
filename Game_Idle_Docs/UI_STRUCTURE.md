# UI 구조 설계

---

## 전체 구조

```
Screen Space Overlay Canvas
│
├── [항상 표시] TopBar                  ← 모든 탭 공통 상단
│   ├── GoldLabel
│   └── ExpBar (레벨 + 경험치)
│
├── [탭 콘텐츠 영역]
│   ├── BattleView      ← 전투 탭
│   ├── CharacterView   ← 캐릭터 탭
│   ├── EnhanceView     ← 강화 탭
│   ├── ShopView        ← 상점 탭
│   └── SettingsView    ← 설정 탭
│
├── [항상 표시] BottomTabBar           ← 하단 탭 네비게이션
│   ├── 전투 탭 버튼
│   ├── 캐릭터 탭 버튼
│   ├── 강화 탭 버튼
│   ├── 상점 탭 버튼
│   └── 설정 탭 버튼
│
└── [조건부 팝업]
    └── OfflineRewardPopup             ← 앱 재진입 시
```

---

## 탭별 상세

### BattleView (전투 탭)

| 요소 | 설명 |
|------|------|
| `StageLabel` | 현재 챕터 / 스테이지 ("Chapter 1 - Stage 3") |
| `SkillButton` × N | 스킬 아이콘 + 쿨타임 오버레이 (Filled Image) |
| `AutoToggleButton` | 자동(AUTO) / 수동(MANUAL) 전환 |

> 스킬 버튼은 전투 탭에서만 표시. 다른 탭으로 이동해도 3D 전투는 계속 진행.

---

### CharacterView (캐릭터 탭)

| 요소 | 설명 |
|------|------|
| 레벨 / 이름 | 현재 레벨 + 캐릭터 이름 |
| 스탯 목록 | ATK / DEF / MaxHP / 공격속도 / 이동속도 / 치명타율 / 치명타 배율 |
| 닫기 (탭 전환) | 탭바로 이동 |

**미결 사항**
- 장비 슬롯 포함 여부 미정

---

### EnhanceView (강화 탭)

| 요소 | 설명 |
|------|------|
| 스킬 카드 목록 | 보유 스킬 리스트 |
| 강화 버튼 | 선택 스킬 강화 (골드 소비) |
| 강화 효과 표시 | 현재 수치 / 강화 후 수치 비교 |

**미결 사항**
- 장비 강화 탭 추후 추가 가능

---

### ShopView (상점 탭)

| 요소 | 설명 |
|------|------|
| 상품 목록 | 구매 가능 아이템/스킬 목록 |
| 구매 버튼 | 골드 소비 |

**미결 사항**
- 상점 판매 목록 기획 미정

---

### SettingsView (설정 탭)

| 요소 | 설명 |
|------|------|
| BGM 볼륨 | 슬라이더 |
| SFX 볼륨 | 슬라이더 |
| HP바 표시 방식 | HUD ↔ 월드스페이스 전환 (추후) |

---

## 공통 요소

### TopBar (상단 — 모든 탭 공통)

| 요소 | 설명 |
|------|------|
| `GoldLabel` | 현재 보유 골드 |
| `ExpBar` | 레벨 + 경험치 진행도 |

### OfflineRewardPopup

| 요소 | 설명 |
|------|------|
| 오프라인 시간 | "XX시간 동안 자리를 비웠습니다" |
| 보상 목록 | 획득 골드 / 경험치 |
| 확인 버튼 | 보상 수령 + 팝업 닫기 |

---

## 컴포넌트

| 컴포넌트 | 역할 |
|----------|------|
| `SkillButton` | 슬롯 연결 + 쿨타임 오버레이 갱신 + 수동 클릭 처리 |
| `ExpBar` | 경험치 fillAmount + 레벨 텍스트 갱신 |
| `TabBar` | 탭 전환 관리 + 현재 활성 탭 표시 |

---

## 파일 구조

```
Assets/Idle_Game/Scripts/
├── Defines/
│   └── UIEnums.cs              ✅ InGameTabType enum
└── UI/
    ├── Tab/
    │   ├── TabButton.cs        ← 잠금/해제 + 선택 상태
    │   └── TabBar.cs           ← 탭 전환 + Lock/Unlock API
    ├── View/
    │   ├── BaseView.cs         ← Show/Hide 기반 클래스
    │   ├── BattleView.cs       ← 스킬 버튼 + 토글 + 스테이지 정보
    │   ├── CharacterView.cs    ← 셸
    │   ├── EnhanceView.cs      ← 셸
    │   ├── ShopView.cs         ← 셸
    │   └── SettingsView.cs     ← 셸
    ├── Battle/
    │   └── SkillButton.cs      ← 쿨타임 오버레이 + 수동 클릭
    ├── TopBar.cs               ← 골드 / 경험치 (더미)
    └── MainCanvas.cs           ← 루트 초기화
```

---

## 구현 순서

1. [x] `UIEnums.cs` — `InGameTabType` 정의
2. [ ] `TabButton` — Inspector 필드: Icon(Image), LockIcon(Image), Label(Text), Button / 메서드: Lock(), Unlock(), SetSelected(bool)
3. [ ] `TabBar` — Inspector 필드: TabButton[] (인덱스 = InGameTabType), BaseView[] (동일 인덱스) / 메서드: SwitchTab(InGameTabType), LockTab(InGameTabType), UnlockTab(InGameTabType) / 기본 Battle 탭 열림, 나머지 잠금 상태로 시작
4. [ ] `BaseView` — Show() / Hide() / IsVisible
5. [ ] `BattleView` — SkillButton[] + AutoToggleButton + StageLabel / Initialize(PlayerController) 호출 필요
6. [ ] `SkillButton` — Initialize(int index, SkillSlot, PlayerController) / Update에서 fillAmount 갱신
7. [ ] `TopBar` — SetGold(int) / SetExp(int current, int max, int level) (더미로 시작)
8. [ ] `MainCanvas` — [SerializeField] TabBar, TopBar, BattleView / Initialize(PlayerController) → MainEntry에서 호출
9. [ ] View 셸 4개 (CharacterView / EnhanceView / ShopView / SettingsView) — BaseView 상속만
10. [ ] `MainEntry` 수정 — `[SerializeField] MainCanvas _canvas` 추가, `_canvas.Initialize(_player)` 호출

---

## 관련 문서

- [스킬 시스템](SKILL_SYSTEM.md)
- [전투 시스템](BATTLE_SYSTEM.md)
- [TODO](TODO.md)
