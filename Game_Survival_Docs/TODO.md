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
- `MainPanel` — 항상 표시되는 HUD (UIPanel 상속, ShowOverlay로 관리)
- `InventoryPanel` — 인벤토리 패널 (UIPanel + IInitializable<Inventory>)
- `InventorySlotElement` — 슬롯 단위 Element (배경/아이콘/수량)
- `InventoryTestButton` — 아이템 추가 테스트용 임시 버튼

#### 씬 초기화
- `SurvivalEntry` — 데이터 로드 + HUD 초기화 진입점

### 🔲 다음 작업

#### 인벤토리 UI 마무리
- [ ] InventorySlotElement 프리팹 완성 (SlotGrid 배치, Inspector 연결)
- [ ] InventoryPanel 프리팹 Inspector 연결 (_slotGrid, _slotElementPrefab)
- [ ] MainPanel Inspector 연결 (_inventoryButton, _player)
- [ ] 플레이 테스트 — 인벤토리 열기/닫기, 아이템 추가 확인

#### 파밍 시스템
- [ ] 자원 오브젝트 (나무, 돌) — 클릭/접근 시 채집
- [ ] 채집 후 인벤토리 자동 추가

#### 전투 시스템
- [ ] 적 AI 기본 구현
- [ ] 플레이어 공격 처리

#### 빌딩 시스템
- [ ] 그리드 기반 건설

---

## 관련 문서

- [기획서](DESIGN.md)
- [레퍼런스 분석](GAME_REFERENCE.md)
