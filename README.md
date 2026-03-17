# Unity Generic Framework

Unity 프로젝트에서 반복적으로 사용되는 패턴과 시스템을 모듈화한 재사용 가능한 제네릭 프레임워크입니다.
각 모듈은 독립적으로 `.unitypackage` 형태로 추출하여 다른 프로젝트에 바로 적용할 수 있습니다.

---

## 모듈 목록

### Core

| 모듈 | 설명 | 상태 |
|------|------|------|
| Singleton | MonoBehaviour 싱글톤 | ⬜ |
| ObjectPool | 제네릭 오브젝트 풀 | ⬜ |
| EventBus | 글로벌 이벤트 시스템 | ⬜ |

### Patterns

| 모듈 | 설명 | 상태 |
|------|------|------|
| StateMachine | 유한 상태 머신 | ⬜ |
| Observer | 옵저버 패턴 | ⬜ |
| Command | 커맨드 패턴 | ⬜ |

### Utils

| 모듈 | 설명 | 상태 |
|------|------|------|
| Timer | 코루틴 기반 타이머 | ⬜ |
| MathExtensions | 수학 유틸리티 | ⬜ |
| CoroutineHelper | 코루틴 헬퍼 | ⬜ |

### 상태 아이콘

- ✅ 완료 (md + unitypackage 존재)
- 🚧 진행 중
- ⬜ 미시작

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 0.1.0 | 2026-03-17 | 프로젝트 초기 설정 |
