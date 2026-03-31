# Unity Generic Framework

Unity 프로젝트에서 반복적으로 사용되는 패턴과 시스템을 모듈화한 재사용 가능한 제네릭 프레임워크입니다.
각 모듈은 독립적으로 `.unitypackage` 형태로 추출하여 다른 프로젝트에 바로 적용할 수 있습니다.

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
| SceneLoader | 씬 전환 + 로딩 화면 | ⬜ |
| AudioManager | BGM/SFX 관리 | ⬜ |
| SaveSystem | JSON/PlayerPrefs 기반 데이터 저장 | ⬜ |
| InputManager | 입력 추상화 (InputSystem 래퍼) | ⬜ |
| ServiceLocator | 의존성 관리 패턴 | ⬜ |

### Patterns

| 모듈 | 설명 | 상태 |
|------|------|------|
| StateMachine | 유한 상태 머신 | ⬜ |
| Observer | 옵저버 패턴 | ⬜ |
| Command | 커맨드 패턴 | ⬜ |
| ObjectFactory | 팩토리 패턴 | ⬜ |
| Strategy | 전략 패턴 | ⬜ |
| Decorator | 데코레이터 패턴 | ⬜ |

### UI

| 모듈 | 설명 | 상태 |
|------|------|------|
| UIManager | UI 스택 관리 | ⬜ |
| PopupSystem | 팝업 열기/닫기 관리 | ⬜ |
| ToastMessage | 알림 메시지 | ⬜ |
| LoadingScreen | 로딩 화면 전환 | ⬜ |
| InfiniteScrollView | 무한 스크롤뷰 (대용량 리스트 최적화) | ⬜ |
| TabSystem | 탭 UI 시스템 | ⬜ |
| DragDropSystem | 드래그 앤 드롭 시스템 | ⬜ |

### Utils

| 모듈 | 설명 | 상태 |
|------|------|------|
| Timer | 코루틴 기반 타이머 | ⬜ |
| MathExtensions | 수학 유틸리티 | ⬜ |
| CoroutineHelper | 코루틴 헬퍼 | ⬜ |
| TransformExtensions | Transform 확장 메서드 | ⬜ |
| VectorExtensions | Vector 연산 확장 메서드 | ⬜ |
| ColorExtensions | Color 유틸리티 | ⬜ |
| WaitCache | WaitForSeconds 캐싱 (GC 최적화) | ⬜ |
| LocalizationSystem | 다국어 지원 | ⬜ |
| AsyncHelper | UniTask 래퍼 (async/await 유틸) | ⬜ |

### 상태 아이콘

- ✅ 완료 (md + unitypackage 존재)
- 🚧 진행 중
- ⬜ 미시작

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 0.6.0 | 2026-03-31 | InGameDataManager 모듈 완료, ExcelClassGenerator 추가 |
| 0.5.0 | 2026-03-31 | DataParser/ExcelToJson 모듈 완료 |
| 0.4.0 | 2026-03-25 | EventBus 모듈 완료 |
| 0.3.0 | 2026-03-25 | ObjectPool 모듈 완료 |
| 0.2.0 | 2026-03-23 | Singleton 모듈 완료 |
| 0.1.0 | 2026-03-17 | 프로젝트 초기 설정 |
