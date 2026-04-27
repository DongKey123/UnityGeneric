# 서바이벌 게임 작업 목록

---

## 설계 — 시스템별 상세 문서 (개발 전 확정 필요)

- [x] LDOE 레퍼런스 분석 (GAME_REFERENCE.md)
- [x] 게임 개요 / 핵심 루프 / 시스템 최소 설계 (DESIGN.md)
- [ ] 맵 / 지역 구조 상세 설계 (MAP.md)
- [ ] 플레이어 스탯 상세 설계 (PLAYER.md)
- [x] 인벤토리 상세 설계 (INVENTORY.md)
- [ ] 크래프팅 상세 설계 (CRAFTING.md)
- [x] 빌딩 / 베이스 건설 상세 설계 (BUILDING.md)
- [x] 전투 상세 설계 (BATTLE.md)
- [x] 파밍 / 자원 수집 설계 (FARMING.md)
- [ ] 오프라인 보상 설계
- [ ] UI 구조 설계 (UI_STRUCTURE.md)

## 미결 사항 (설계 중 확정 예정)

- [ ] 씬 구조 — 지역별 씬 분리 vs 단일 씬
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
- `InventoryPanel` — 인벤토리 패널 (UIPanel + IInitializable\<Inventory\>, ScrollView)
- `InventorySlotElement` — 슬롯 단위 Element (배경/아이콘/수량)
- `InventoryTestButton` — 아이템 추가 테스트용 임시 버튼 *(테스트 완료 후 제거)*

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

#### 파밍 시스템 — 추후 기능
- [ ] 자동 채집 — 범위 안에 머물면 자동으로 채집 진행

#### 전투 시스템
- [ ] `EnemyData` — 적 데이터 클래스 + `Enemy.json` 샘플
- [ ] `IDamageable` — Player / Enemy 공통 인터페이스
- [ ] `Enemy` — HP 관리, FSM (Idle/Chase/Attack/Dead), NavMesh 이동, 군집 반응
- [ ] `EnemySpawner` — 테이블 기반 스폰
- [ ] MainPanel 공격 버튼 — 범위 내 적 감지 시 활성화, 자동 타겟
- [ ] 적 터치 공격 입력 처리
- [ ] 드롭 처리 — EnemyDeadEvent → 인벤토리 자동 추가 + Toast

#### 빌딩 시스템
- [ ] 그리드 기반 건설

---

## 관련 문서

- [기획서](DESIGN.md)
- [레퍼런스 분석](GAME_REFERENCE.md)
