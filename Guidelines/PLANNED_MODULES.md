# Planned Modules

개발 예정인 Framework 모듈 목록입니다.

---

## Core

| 모듈 | 설명 | 완료 |
|------|------|------|
| `Singleton<T>` | MonoBehaviour 싱글톤 | [x] |
| `ObjectPool<T>` | 제네릭 오브젝트 풀 | [x] |
| `EventBus` | 글로벌 이벤트 시스템 | [x] |
| `DataParser/ExcelToJson` | Excel 파일을 읽어 JSON으로 변환 저장 (ExcelDataReader, Newtonsoft.Json 의존) | [x] |
| `InGameDataManager` | JSON 로드 + 키 컬럼 기준 Dictionary 캐싱, 런타임 게임 데이터 관리 | [x] |
| `SceneLoader` | 씬 전환 + 로딩 화면 | [x] |
| `AudioManager` | BGM/SFX 관리 | [ ] |
| `SaveSystem` | JSON/PlayerPrefs 기반 데이터 저장 | [x] |
| `InputManager` | 입력 추상화 (InputSystem 래퍼) | [ ] |
| `ServiceLocator` | 의존성 관리 패턴 | [ ] |

## Patterns

| 모듈 | 설명 | 완료 |
|------|------|------|
| `StateMachine<T>` | 유한 상태 머신 | [ ] |
| `Observer` | 옵저버 패턴 | [ ] |
| `Command` | 커맨드 패턴 | [ ] |
| `ObjectFactory<T>` | 팩토리 패턴 | [ ] |
| `Strategy<T>` | 전략 패턴 | [ ] |
| `Decorator` | 데코레이터 패턴 | [ ] |

## UI

| 모듈 | 설명 | 완료 |
|------|------|------|
| `UIManager` | UI 스택 관리 | [ ] |
| `PopupSystem` | 팝업 열기/닫기 관리 | [ ] |
| `ToastMessage` | 알림 메시지 | [ ] |
| `LoadingScreen` | 로딩 화면 전환 | [ ] |
| `InfiniteScrollView` | 무한 스크롤뷰 (대용량 리스트 최적화) | [ ] |
| `TabSystem` | 탭 UI 시스템 | [ ] |
| `DragDropSystem` | 드래그 앤 드롭 시스템 | [ ] |

## Utils

| 모듈 | 설명 | 완료 |
|------|------|------|
| `Timer` | 코루틴 기반 타이머 | [ ] |
| `MathExtensions` | 수학 유틸리티 | [ ] |
| `CoroutineHelper` | 코루틴 헬퍼 | [ ] |
| `TransformExtensions` | Transform 확장 메서드 | [ ] |
| `VectorExtensions` | Vector 연산 확장 메서드 | [ ] |
| `ColorExtensions` | Color 유틸리티 | [ ] |
| `WaitCache` | WaitForSeconds 캐싱 (GC 최적화) | [ ] |
| `LocalizationSystem` | 다국어 지원 | [ ] |
| `AsyncHelper` | UniTask 래퍼 (async/await 유틸) | [ ] |

---

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
| `TweenHelper` | DOTween 래퍼 (애니메이션 유틸) |
| `ReactiveProperty<T>` | 값 변경 감지 프로퍼티 |
