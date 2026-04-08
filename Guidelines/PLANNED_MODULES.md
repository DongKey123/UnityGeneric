# Planned Modules

개발 예정인 Framework 모듈 목록입니다.

### 중요도

- ★★★ 필수 — 없으면 기본 동작에 문제가 생기거나 심사 반려 가능
- ★★☆ 권장 — 대부분의 프로젝트에서 필요
- ★☆☆ 선택 — 장르/규모에 따라 필요

---

## Core

| 모듈 | 설명 | 완료 |
|------|------|------|
| `Singleton<T>` | ★★★ MonoBehaviour 싱글톤 | [x] |
| `ObjectPool<T>` | ★★★ 제네릭 오브젝트 풀 | [x] |
| `EventBus` | ★★★ 글로벌 이벤트 시스템 | [x] |
| `DataParser/ExcelToJson` | ★★☆ Excel 파일을 읽어 JSON으로 변환 저장 (ExcelDataReader, Newtonsoft.Json 의존) | [x] |
| `InGameDataManager` | ★★★ JSON 로드 + 키 컬럼 기준 Dictionary 캐싱, 런타임 게임 데이터 관리 | [x] |
| `SceneLoader` | ★★★ 씬 전환 + 로딩 화면 | [x] |
| `AudioManager` | ★★★ BGM/SFX 관리 | [x] |
| `SaveSystem` | ★★★ JSON/PlayerPrefs 기반 데이터 저장 | [x] |
| `InputManager` | ★★★ 입력 추상화 (InputSystem 래퍼) | [x] |

## Patterns

| 모듈 | 설명 | 완료 |
|------|------|------|
| `StateMachine<T>` | ★★★ 유한 상태 머신 | [ ] |
| `Observer` | ★★☆ 옵저버 패턴 | [ ] |
| `Command` | ★★☆ 커맨드 패턴 | [ ] |
| `ObjectFactory<T>` | ★★☆ 팩토리 패턴 | [ ] |
| `Strategy<T>` | ★★☆ 전략 패턴 | [ ] |
| `Decorator` | ★☆☆ 데코레이터 패턴 | [ ] |

## UI

| 모듈 | 설명 | 플랫폼 | 완료 |
|------|------|--------|------|
| `UIManager` | ★★★ UI 패널 2레이어 관리 (Default 스택 / Overlay) | 공통 | [x] |
| `SafeAreaFitter` | ★★★ 노치/펀치홀/홈 인디케이터 영역 자동 회피 | 모바일 | [x] |
| `UITransitionSystem` | ★★★ Open/Close 시 페이드/슬라이드 트랜지션 (AsyncHelper 완료 후 작업) | 공통 | [ ] |
| `LocalizationSystem` | ★★★ 다국어 텍스트/폰트 런타임 교체 | 공통 | [ ] |
| `BottomSheet` | ★★★ 하단 슬라이드업 패널 (half/full/hidden 스냅) | 모바일 | [ ] |
| `CommonPopupManager` | ★★★ 공용 팝업 관리 (OneButton / TwoButton / Dim / 스택) | 공통 | [x] |
| `ToastManager` | ★★★ 알림 메시지 (UIManager 독립, 별도 싱글톤) | 공통 | [x] |
| `ConfirmDialogBuilder` | ★★☆ 확인/취소 팝업 빌더 API (IInitializable 연동) | 공통 | [ ] |
| `SwipeGestureDetector` | ★★☆ 스와이프 방향 판정, 핀치줌 감지 | 모바일 | [ ] |
| `KeyboardAvoidance` | ★★☆ 소프트 키보드 올라올 때 InputField 자동 이동 | 모바일 | [ ] |
| `ResolutionScaler` | ★★☆ 다양한 종횡비(16:9~21:9) 동적 대응 | 공통 | [ ] |
| `TweenHelper` | ★★☆ 버튼 탭 피드백, 숫자 카운트업, HP바 애니메이션 (DOTween 의존) | 공통 | [ ] |
| `LoadingScreen` | ★★☆ 로딩 화면 전환 | 공통 | [ ] |
| `InfiniteScrollView` | ★★☆ 무한 스크롤뷰 (대용량 리스트 최적화) | 공통 | [ ] |
| `TabSystem` | ★★☆ 탭 UI 시스템 | 공통 | [ ] |
| `JoystickUI` | ★★☆ 버추얼 조이스틱 고정형/다이나믹 (액션RPG 필수) | 모바일 | [ ] |
| `HapticFeedback` | ★☆☆ iOS/Android 진동 패턴 통합 래퍼 | 모바일 | [ ] |
| `DragDropSystem` | ★☆☆ 드래그 앤 드롭 시스템 | 공통 | [ ] |
| `UIFocusManager` | ★☆☆ 키보드/게임패드 포커스 네비게이션 | 윈도우 | [ ] |
| `UIAccessibilityHelper` | ★☆☆ 폰트 크기, 고대비, 색맹 지원 | 공통 | [ ] |

## Utils

| 모듈 | 설명 | 완료 |
|------|------|------|
| `Coroutine` (WaitCache / CoroutineRunner / CoroutineTimer) | ★★★ 코루틴 유틸리티 모음 (GC 최적화 캐싱, 외부 실행, 타이머) | [x] |
| `Extensions` (TransformExtensions / VectorExtensions / ColorExtensions) | ★★★ Unity 타입 확장 메서드 모음 | [x] |
| `Math` (MathExtensions) | ★★☆ 수학 유틸리티 | [x] |
| `AsyncHelper` | ★★★ UniTask 래퍼 (async/await 유틸) | [ ] |
| `LocalizationSystem` | ★★★ 다국어 지원 | [ ] |

---

## 고민 중 / 추가 예정

### 고급 UI

| 모듈 | 설명 |
|------|------|
| `VirtualList` | 수천 개 아이템도 일부만 렌더링하는 리스트 |
| `RadialMenu` | 원형 메뉴 (액션RPG 등) |
| `MinimapSystem` | 미니맵 렌더링 시스템 |
| `DamageNumberSystem` | 데미지 숫자 연출 풀링 |

### AI / 게임플레이

| 모듈 | 설명 |
|------|------|
| `BehaviorTree` | AI 행동 트리 |
| `AStarPathfinding` | 격자 기반 길찾기 |
| `DialogueSystem` | 대화/스크립트 시스템 |
| `QuestSystem` | 퀘스트 관리 |
| `BuffDebuffSystem` | 버프/디버프 스택 관리 |
| `InventorySystem<T>` | 제네릭 인벤토리 |

### 렌더링 / 최적화

| 모듈 | 설명 |
|------|------|
| `ChunkSystem` | 무한 맵 청크 로딩/언로딩 |
| `ShaderPropertyCache` | Shader.PropertyToID 캐싱 |
| `LODController` | 거리 기반 LOD 전환 |

### 비동기 / 트위닝

| 모듈 | 설명 |
|------|------|
| `ReactiveProperty<T>` | 값 변경 감지 프로퍼티 |

### 도입 여부 미결정

| 항목 | 설명 | 고민 포인트 |
|------|------|------------|
| `UIManager — Addressables 전환` | Resources.Load 동기 로드를 Addressables 비동기 로드로 교체 | 프로젝트 규모가 작으면 오버엔지니어링. 대형 프로젝트에서는 빌드 용량 및 메모리 관리에 유리 |
