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
| `InGameDataManager` | JSON 로드 + 키 컬럼 기준 Dictionary 캐싱, 런타임 게임 데이터 관리 | [ ] |
| `SceneLoader` | 씬 전환 + 로딩 화면 | [ ] |
| `AudioManager` | BGM/SFX 관리 | [ ] |
| `SaveSystem` | JSON/PlayerPrefs 기반 데이터 저장 | [ ] |
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

## 진행 중 모듈 — 남은 작업

### DataParser/ExcelToJson

기본 변환 기능 구현 완료. 아래 기능 추가 예정.

| 기능 | 설명 | 완료 |
|------|------|------|
| 단일 파일 변환 | 파일 선택 후 JSON 출력 | [x] |
| 시트별 JSON 분리 출력 | 각 시트 → 별도 JSON 파일 | [x] |
| 타입 행 | int / float / bool / string / int[] / float[] / string[] | [x] |
| 주석 행 | `#` 으로 시작하는 행 무시 | [x] |
| 빈 행 무시 | 완전히 비어있는 행 건너뜀 | [x] |
| 주석 시트 무시 | `#` 으로 시작하는 시트명 건너뜀 | [x] |
| Pretty Print 옵션 | 들여쓰기 ON/OFF | [x] |
| enum 타입 지원 | string 타입과 동일하게 처리 (별칭 추가) | [x] |
| 폴더 일괄 변환 | 폴더 내 모든 .xlsx 한 번에 변환 | [x] |
| 출력 경로 기억 | 마지막 설정을 EditorPrefs에 저장 | [x] |
| 자동 변환 | Excel 파일 변경 감지 시 자동 변환 | [x] |
| 특정 시트만 선택 | 시트 목록 체크박스로 변환 대상 선택 | [x] |
| 변환 결과 미리보기 | 변환 후 JSON 구조 + 시트별 성공/실패 로그를 창 내에서 확인 | [x] |

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
