# 서바이벌 게임 작업 목록

---

## 설계 — 시스템별 상세 문서 (개발 전 확정 필요)

- [x] LDOE 레퍼런스 분석 (GAME_REFERENCE.md)
- [x] 게임 개요 / 핵심 루프 / 시스템 최소 설계 (DESIGN.md)
- [x] 맵 / 지역 구조 상세 설계 (MAP.md)
- [ ] 플레이어 스탯 상세 설계 (PLAYER.md)
- [x] 인벤토리 상세 설계 (INVENTORY.md)
- [ ] 크래프팅 상세 설계 (CRAFTING.md)
- [x] 빌딩 / 베이스 건설 상세 설계 (BUILDING.md)
- [x] 전투 상세 설계 (BATTLE.md)
- [x] 파밍 / 자원 수집 설계 (FARMING.md)
- [ ] 오프라인 보상 설계
- [ ] UI 구조 설계 (UI_STRUCTURE.md)

## 미결 사항 (설계 중 확정 예정)

- [x] 씬 구조 — 단일 씬 (메인) + 이벤트 씬 분리로 확정
- [ ] 레벨 시스템 포함 여부
- [ ] PvP 레이드 포함 여부
- [ ] 장기 목표 (최종 콘텐츠)
- [ ] 세계관 / 적 명칭

---

## 구현

### ✅ 완료

#### 캐릭터 이동
- `SurvivalInputManager` — MobileInputManager 상속, 조이스틱 방향 관리
- `VirtualJoystick` — 좌하단 가상 조이스틱 UI, LDOE 방식
- `PlayerController` — Rigidbody 기반 이동, 인벤토리 소유
- `PlayerCamera` — 고정 오프셋 추적 카메라

#### 데이터
- `SurvivalItemData` — 아이템 데이터 클래스 (Resource/Equipment/Consumable)
- `SurvivalDataLoader` — 서바이벌 게임 전용 데이터 로더
- `ItemEnums` — ItemCategory, EquipmentSlotType 열거형
- `Resources/Data/Item.json` — 샘플 아이템 6종 (나무/돌/철조각/나무도끼/나무검/붕대)

#### 인벤토리 (백엔드)
- `Inventory` — 슬롯 수 + 무게 이중 제한, TryAdd/TryRemove/GetCount/HasItem
- `InventorySlot` — 아이템 데이터, 수량, 내구도 관리

#### 인벤토리 UI
- `MainPanel` — 항상 표시되는 HUD (UIPanel 상속, Default Layer)
- `InventoryPanelData` — Inventory + EquipmentSlots 전달용 데이터 클래스
- `InventoryPanel` — 3단 레이아웃 (장비 슬롯 | 아이템 그리드 | 상세 패널), IInitializable\<InventoryPanelData\>
- `InventorySlotElement` — 슬롯 단위 Element (선택/장착 glow, 배지, 아이콘 동적 로드)
- `EquipmentSlotsSubPanel` — 캐릭터 실루엣 + 6슬롯, 장착 아이콘 반영
- `ItemDetailSubPanel` — 아이콘/이름/카테고리/설명/무게/내구도 바/티어 표시
- `InventoryTestButton` — 아이템 추가 테스트용 임시 버튼 *(테스트 완료 후 제거)*
- `Resources/Data/Item.json` — icon_path 6종 입력 완료
- `Resources/Sprites/Items/` — 아이템 아이콘 14종 배치 (Resources.Load 경로)

#### 맵 / 지역 설계
- `MAP.md` — 단일 씬 구조, CoC식 무한 영역 확장, 자원 오토스폰 테이블, 이벤트 씬 분리

#### 에디터 툴
- `InventoryPanelSetup` — InventoryPanel / InventorySlotElement 프리팹 자동 생성 (앵커 절대좌표 방식)
- `FontSetup` — NanumGothic-Bold TMP FontAsset 생성 + 전체 UI 프리팹 적용
- `PanelRectFixer` — Resources/UI 하위 모든 프리팹 루트 RT 전체화면 Stretch 초기화

#### 씬 초기화
- `SurvivalEntry` — 데이터 로드 + HUD 초기화 + 자원 스폰 진입점

#### 파밍 시스템
- `ResourceData` — 자원 데이터 클래스 (resource_id, item_id, durability_max, respawn_time, drop_count, prefab_path)
- `Resource.json` — 자원 데이터 테이블 (Wood/Stone 2종)
- `HarvestEvents` — EventBus 이벤트 구조체 (HarvestRangeEntered/Exited, ResourceHarvested)
- `ResourceObject` — 자원 오브젝트 (내구도, 범위 감지, 채집 처리, 쿨타임 후 재생성, 리스폰 범위 재진입 처리)
- `ResourceSpawner` — 원점 기준 반경 20f 내 랜덤 스폰 (맵 시스템 완성 전 임시)
- `HarvestButton` — MainPanel 채집 버튼 (범위 진입 시 활성화, 이탈/채집 완료 시 비활성화)
- `ToastPanel.prefab` — 채집 완료 토스트 알림 (화면 중앙, 페이드 인/아웃)
- `Resource_Tree`, `Resource_Rock` 프리팹 생성 및 씬 배치 완료

---

### 🔲 예정

#### 인벤토리 UI — 장비 시스템 연동 ✅

- [x] `InventoryPanel` 리디자인 — 3단 레이아웃 (장비 슬롯 | 아이템 그리드 | 상세 패널)
- [x] `EquipmentSlotsSubPanel` — 캐릭터 실루엣 + 6슬롯 (Weapon/Tool/Head/Chest/Legs/Boots), 장착 부위 glow 표시
- [x] `InventorySlotElement` 상태 추가 — 선택(금색 테두리) / 장착중(초록 배지)
- [x] `ItemDetailSubPanel` — 아이콘, 이름, 카테고리, 설명, 무게, 내구도 바, 티어
- [x] 액션 버튼 — 사용(Consumable), 장착/해제(Equipment), 버리기 조건부 활성
- [x] `InventoryPanel` ↔ `EquipmentSlots` 연동 — 장착/해제 버튼 → `PlayerController.Equipment` 호출

#### 파밍 시스템 — 추후 기능
- [ ] 자동 채집 — 범위 안에 머물면 자동으로 채집 진행

#### 크래프팅 시스템 — 코드
- [x] `RecipeData` — 레시피 데이터 클래스 + `Recipe.json` (Wooden Axe / Wooden Sword / Bandage 3종)
- [x] `CraftingSystem` — 재료 확인(CanCraft) + 소모 후 지급(TryCraft) 정적 클래스
- [x] `CraftingPanel` — 레시피 목록 UI, 재료 현황 표시, Inventory.OnChanged 구독으로 버튼 자동 갱신
- [x] `CraftingPanel.prefab` — RecipeListRoot + RecipeButtonTemplate 구성 및 스크립트 필드 연결
- [x] `SurvivalDataLoader` — RecipeData 로드 추가
- [x] `MainPanel` — Craft 버튼 추가 (_craftButton)

#### 크래프팅 시스템 — Unity 에디터 작업
- [x] MainPanel에 Craft 버튼 추가 및 `_craftButton` 필드 연결
- [x] RecipeListRoot에 VerticalLayoutGroup 추가 (Child Alignment: Upper Center, Spacing: 15, Control Child Size Width: ✅)

#### 전투 시스템 — 코드
- [x] `EnemyData` — 적 데이터 클래스 + `Enemy.json` (Zombie/Wolf 2종)
- [x] `IDamageable` — Player / Enemy 공통 인터페이스
- [x] `PlayerController` — IDamageable 구현 (HP, TakeDamage)
- [x] `EnemyEvents` — EnemyDied / EnemyAttacked 이벤트 구조체
- [x] `Enemy` — HP 관리, FSM (Idle/Chase/Attack/Dead), NavMesh 이동, 군집 반응
- [x] `EnemySpawner` — 테이블 기반 스폰, 사망 시 인벤토리 드롭 + Toast
- [x] `SurvivalDataLoader` — EnemyData 로드 추가
- [x] `SurvivalEntry` — EnemySpawner 연동
- [x] MainPanel 공격 버튼 — 범위 내 적 감지 시 활성화, 자동 타겟
- [x] 적 터치 공격 입력 처리

#### 전투 시스템 — Unity 에디터 작업
- [x] `Enemy_Zombie` 프리팹 생성 (`Resources/Prefabs/Combat/`)
- [x] `Enemy_Wolf` 프리팹 생성 (동일 구조)
- [x] EnemySpawner GameObject 씬 배치
- [x] SurvivalEntry `_enemySpawner` 필드 연결
- [x] NavMesh Bake
- [x] MainPanel `_attackButton` 필드 연결 (Inspector)

#### 빌딩 시스템 — 코드
- [x] `BuildingData` — 건물 데이터 클래스 + `Building.json` (Wood Floor 1종, grid 1x1, 재료 테이블)
- [x] `BuildingGrid` — 정수 좌표 그리드 (WorldToCell/CellToWorld, 점유 등록/해제)
- [x] `BuildingPlacer` — 배치 모드 관리, 고스트 미리보기 (녹/적 색상), 자원 소모 후 건물 생성
- [x] `PlacedBuilding` — 배치된 건물 컴포넌트, 철거 시 재료 50% 반환
- [x] `BuildModePanel` — 빌드 모드 UI 패널 (선택 화면 ↔ 배치 화면 전환)
- [x] `SurvivalDataLoader` — BuildingData 로드 추가
- [x] `SurvivalEntry` — BuildingGrid / BuildingPlacer 필드 추가
- [x] `BuildModePanel.prefab` — SelectionView + BuildingListRoot + BuildingButtonTemplate + PlacementView + PlaceButton 구성 및 스크립트 필드 전체 연결

#### 빌딩 시스템 — Unity 에디터 작업
- [x] `BuildingGrid` GameObject 씬 배치
- [x] `BuildingPlacer` GameObject 씬 배치
- [x] `SurvivalEntry` `_buildingGrid` / `_buildingPlacer` 필드 연결
- [x] MainPanel에 Build 버튼 추가 및 `_buildButton` 필드 연결
- [x] 배치 동작 확인

---

## 관련 문서

- [기획서](DESIGN.md)
- [레퍼런스 분석](GAME_REFERENCE.md)
