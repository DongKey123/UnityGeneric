# Unity Generic Framework

Unity 프로젝트에서 반복적으로 사용되는 패턴과 시스템을 모듈화한 재사용 가능한 제네릭 프레임워크입니다.

---

## 모듈 목록

### Core

| 모듈 | 설명 | 상태 |
|------|------|------|
| Singleton | MonoBehaviour 싱글톤 | ✅ |
| ObjectPool | 제네릭 오브젝트 풀 | ✅ |
| EventBus | 글로벌 이벤트 시스템 | ✅ |
| DataParser/ExcelToJson | Excel 파일을 읽어 JSON으로 변환 저장 | ✅ |
| InGameDataManager | JSON 로드 + 키 컬럼 기준 Dictionary 캐싱 | ✅ |
| SceneLoader | 씬 전환 + 로딩 화면 | ✅ |
| AudioManager | BGM/SFX 관리 | ✅ |
| SaveSystem | JSON/PlayerPrefs 기반 데이터 저장 | ✅ |
| InputManager | 입력 추상화 (InputSystem 래퍼) | ✅ |

### Patterns

| 모듈 | 설명 | 상태 |
|------|------|------|
| StateMachine | 유한 상태 머신 | ✅ |

### UI

| 모듈 | 설명 | 상태 |
|------|------|------|
| UIManager | UI 패널 2레이어 관리 (Default 스택 / Overlay) | ✅ |
| SafeAreaFitter | 노치/펀치홀/홈 인디케이터 Safe Area 자동 적용 | ✅ |
| ToastManager | 자동으로 사라지는 토스트 알림 메시지 | ✅ |
| CommonPopupManager | 공용 팝업 관리 (OneButton / TwoButton / Dim / 스택) | ✅ |
| LocalizationSystem | 다국어 텍스트/폰트 런타임 교체 | ✅ |

### Utils

| 모듈 | 설명 | 상태 |
|------|------|------|
| Coroutine (WaitCache / CoroutineRunner / CoroutineTimer) | 코루틴 유틸리티 모음 | ✅ |
| Extensions (TransformExtensions / VectorExtensions / ColorExtensions) | Unity 타입 확장 메서드 모음 | ✅ |
| Math (MathExtensions) | 수학 유틸리티 | ✅ |
| AsyncHelper | UniTask 래퍼 (async/await 유틸) | ✅ |
| LocalizationSystem | 다국어 지원 | ✅ |

### 상태 아이콘

- ✅ 완료
- 🚧 진행 중
- ⬜ 미시작

---

## 샘플 게임

프레임워크 모듈 연동을 검증하는 샘플 게임입니다.

### 서바이벌 게임 🟢 진행 중

| 항목 | 내용 |
|------|------|
| 장르 | 서바이벌 (Last Day on Earth 스타일) |
| 시점 | 3D 쿼터뷰 |
| 경로 | `Assets/Game_Survival/` |
| 기획서 | [DESIGN.md](Game_Survival_Docs/DESIGN.md) |
| 작업 목록 | [TODO.md](Game_Survival_Docs/TODO.md) |

**구현 완료 시스템**

| 시스템 | 주요 스크립트 |
|--------|--------------|
| 캐릭터 이동 | `PlayerController`, `SurvivalInputManager`, `VirtualJoystick`, `PlayerCamera` |
| 데이터 | `SurvivalItemData`, `ResourceData`, `RecipeData`, `EnemyData`, `BuildingData`, `SurvivalDataLoader` |
| 인벤토리 | `Inventory`, `InventorySlot` |
| 인벤토리 UI | `MainPanel`, `InventoryPanel`, `InventorySlotElement` |
| 파밍 | `ResourceObject`, `ResourceSpawner`, `HarvestEvents`, `ToastPanel` |
| 크래프팅 | `CraftingSystem`, `CraftingPanel` |
| 전투 | `Enemy`, `EnemySpawner`, `EnemyEvents`, `IDamageable` |
| 빌딩 | `BuildingGrid`, `BuildingPlacer`, `PlacedBuilding`, `BuildModePanel` |
| 씬 초기화 | `SurvivalEntry` |

**아트 리소스**

| 팩 | 라이선스 | 경로 | 내용 |
|----|----------|------|------|
| [Kenney — Survival Kit](https://kenney.nl/assets/survival-kit) | CC0 1.0 | `Art/Models/Environment/` | 환경 FBX 80종 (barrel, chest, campfire, fence, rock, tree, structure, workbench, tool 등) |
| [Kenney — Animated Characters: Survivors](https://kenney.nl/assets/animated-characters-1) | CC0 1.0 | `Art/Models/Characters/`, `Art/Animations/Characters/`, `Art/Textures/Characters/` | `characterMedium.fbx`, 애니메이션 3종 (idle/run/jump), 스킨 PNG 4종 (survivor×2, zombie×2) |

### Idle RPG ⏸ 보류

| 항목 | 내용 |
|------|------|
| 장르 | 방치형 RPG |
| 시점 | 3D 쿼터뷰 |
| 기획서 | [IDLE_RPG_DESIGN.md](Game/IDLE_RPG_DESIGN.md) |

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 2.3.0 | 2026-04-24 | 서바이벌 게임 파밍 시스템 완료 (ResourceObject / ResourceSpawner / HarvestEvents / ToastPanel) |
| 2.2.0 | 2026-04-24 | 서바이벌 게임 인벤토리 UI 완료 (MainPanel / InventoryPanel / InventorySlotElement) |
| 2.1.0 | 2026-04-24 | 서바이벌 게임 기초 시스템 완료 (PlayerController / Inventory / SurvivalDataLoader / SurvivalEntry) |
| 2.0.0 | 2026-04-08 | StateMachine 모듈 완료 (제네릭 FSM, IState / BaseState / OnStateChanged) |
| 1.9.0 | 2026-04-08 | AsyncHelper 모듈 완료 (UniTask 래퍼, Delay / Frame / Condition / Cancellation) |
| 1.8.0 | 2026-04-08 | LocalizationSystem 모듈 완료 (다국어 JSON 로드, LocalizedText 자동 갱신) |
| 1.7.0 | 2026-04-08 | CommonPopupManager 모듈 완료 (OneButtonPopup / TwoButtonPopup / Dim / 스택 관리) |
| 1.6.0 | 2026-04-08 | ToastManager 모듈 완료 (큐 기반 토스트 알림, Default/Success/Warning/Error 타입) |
| 1.5.0 | 2026-04-08 | SafeAreaFitter 모듈 완료 (노치/펀치홀/홈 인디케이터 Safe Area 자동 적용) |
| 1.4.0 | 2026-04-03 | UIManager 모듈 완료 (2레이어 Default/Overlay, CanClose/CloseOnBack, 이벤트/레지스트리) |
| 1.3.0 | 2026-04-03 | Math 모듈 완료 (MathExtensions) |
| 1.2.0 | 2026-04-03 | Extensions 모듈 완료 (TransformExtensions / VectorExtensions / ColorExtensions) |
| 1.1.0 | 2026-04-03 | Coroutine Utils 모듈 완료 (WaitCache / CoroutineRunner / CoroutineTimer) |
| 1.0.0 | 2026-04-03 | AudioManager 모듈 완료 (BGM/SFX) |
| 0.9.0 | 2026-04-01 | InputManager 모듈 완료 (Desktop/Mobile) |
| 0.8.0 | 2026-04-01 | SaveSystem 모듈 완료 |
| 0.7.0 | 2026-04-01 | SceneLoader 모듈 완료 |
| 0.6.0 | 2026-03-31 | InGameDataManager 모듈 완료, ExcelClassGenerator 추가 |
| 0.5.0 | 2026-03-31 | DataParser/ExcelToJson 모듈 완료 |
| 0.4.0 | 2026-03-25 | EventBus 모듈 완료 |
| 0.3.0 | 2026-03-25 | ObjectPool 모듈 완료 |
| 0.2.0 | 2026-03-23 | Singleton 모듈 완료 |
| 0.1.0 | 2026-03-17 | 프로젝트 초기 설정 |
