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
| StateMachine | 유한 상태 머신 | ⬜ |
| Observer | 옵저버 패턴 | ⬜ |
| Command | 커맨드 패턴 | ⬜ |
| ObjectFactory | 팩토리 패턴 | ⬜ |
| Strategy | 전략 패턴 | ⬜ |
| Decorator | 데코레이터 패턴 | ⬜ |

### UI

| 모듈 | 설명 | 상태 |
|------|------|------|
| UIManager | UI 패널 2레이어 관리 (Default 스택 / Overlay) | ✅ |
| SafeAreaFitter | 노치/펀치홀/홈 인디케이터 Safe Area 자동 적용 | ✅ |
| ToastManager | 자동으로 사라지는 토스트 알림 메시지 | ✅ |
| UITransitionSystem | Open/Close 페이드/슬라이드 트랜지션 | ⬜ |
| LocalizationSystem | 다국어 텍스트/폰트 런타임 교체 | ✅ |
| BottomSheet | 하단 슬라이드업 패널 | ⬜ |
| CommonPopupManager | 공용 팝업 관리 (OneButton / TwoButton / Dim / 스택) | ✅ |
| ConfirmDialogBuilder | 확인/취소 팝업 빌더 API | ⬜ |
| SwipeGestureDetector | 스와이프 방향 판정, 핀치줌 감지 | ⬜ |
| KeyboardAvoidance | 소프트 키보드 올라올 때 InputField 자동 이동 | ⬜ |
| ResolutionScaler | 다양한 종횡비 동적 대응 | ⬜ |
| TweenHelper | 버튼 탭 피드백, 숫자 카운트업, HP바 애니메이션 | ⬜ |
| LoadingScreen | 로딩 화면 전환 | ⬜ |
| InfiniteScrollView | 무한 스크롤뷰 (대용량 리스트 최적화) | ⬜ |
| TabSystem | 탭 UI 시스템 | ⬜ |
| JoystickUI | 버추얼 조이스틱 (모바일) | ⬜ |
| HapticFeedback | iOS/Android 진동 패턴 통합 래퍼 | ⬜ |
| DragDropSystem | 드래그 앤 드롭 시스템 | ⬜ |
| UIFocusManager | 키보드/게임패드 포커스 네비게이션 | ⬜ |
| UIAccessibilityHelper | 폰트 크기, 고대비, 색맹 지원 | ⬜ |

### Utils

| 모듈 | 설명 | 상태 |
|------|------|------|
| Coroutine (WaitCache / CoroutineRunner / CoroutineTimer) | 코루틴 유틸리티 모음 | ✅ |
| Extensions (TransformExtensions / VectorExtensions / ColorExtensions) | Unity 타입 확장 메서드 모음 | ✅ |
| Math (MathExtensions) | 수학 유틸리티 | ✅ |
| LocalizationSystem | 다국어 지원 | ⬜ |
| AsyncHelper | UniTask 래퍼 (async/await 유틸) | ⬜ |

### 상태 아이콘

- ✅ 완료
- 🚧 진행 중
- ⬜ 미시작

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
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
