# Claude Instructions

이 프로젝트는 Unity용 재사용 가능한 제네릭 프레임워크입니다.
아래 가이드라인 문서를 반드시 참고하여 코드를 작성하세요.

## 모듈 완료 시 필수 작업

모듈 구현이 완료될 때마다 아래 항목을 반드시 업데이트하세요.

1. 해당 모듈 폴더에 `<ModuleName>.md` 작성 ([문서 템플릿 참고](Guidelines/DOCS_STYLE.md))
2. `README.md` 모듈 목록에서 해당 모듈 상태를 ✅ 로 변경 — README의 모든 섹션(Core/Patterns/UI/Utils)에 해당 모듈이 누락 없이 등록되어 있는지 함께 확인
3. `README.md` 변경 이력에 버전, 날짜, 내용 추가
4. 작업된 모든 파일의 XML 주석 검토 — `&lt;` / `&gt;` 엔티티 노출, `<param>` / `<returns>` 누락 여부 확인 후 수정

---

## 프레임워크 수정 시 필수 절차

게임 코드 작업 중 프레임워크(`Assets/Scripts/Framework/`) 수정이 발생하면 아래 순서를 따르세요.

1. **프레임워크 변경 사항을 먼저 커밋 · 푸시** — 게임 코드 작업이 끝나지 않았더라도 프레임워크 변경분만 별도 커밋으로 먼저 올립니다. 게임 코드는 작업 완료 후 한꺼번에 올려도 무방합니다.
2. **커밋 메시지에 변경된 API를 명시** — 어떤 메서드/클래스가 추가·변경·제거되었는지 적습니다.
3. **CLAUDE.md 하단 "프레임워크 변경 이력"에 한 줄 추가** — 날짜, 변경 내용, 영향 범위를 기록합니다.
4. **이 프로젝트를 참조하는 다른 프로젝트가 있다면 최신화 필요** — 프레임워크 변경 후 의존 프로젝트에서 패키지 업데이트가 필요할 수 있습니다.

---

## 프레임워크 변경 이력

| 날짜 | 변경 내용 | 영향 범위 |
|------|-----------|-----------|
| 2026-04-16 | `InGameDataManager.GetAll<TValue>()` 추가 — Dictionary 캐시 전체 값 반환 | `LoadAsDictionary`로 로드된 데이터 전체 조회가 필요한 모든 곳 |

---

## 현재 진행 중인 게임

| 게임 | 상태 | 문서 |
|------|------|------|
| 서바이벌 게임 (Last Day on Earth 스타일) | 🟢 진행 중 | [기획서](Game_Survival_Docs/DESIGN.md) · [TODO](Game_Survival_Docs/TODO.md) |
| Idle RPG | ⏸ 보류 | [TODO](Game_Idle_Docs/TODO.md) |

> 게임 코드 작업 시 해당 게임의 TODO.md 및 설계 문서를 우선 참고하세요.
> 프레임워크(`Assets/Scripts/Framework/`) 수정이 발생하면 위 **프레임워크 수정 시 필수 절차**를 따르세요.

---

## 게임 코드 작업 시 필수 원칙

게임 코드를 작성하기 전에 **반드시 프레임워크를 먼저 확인**하세요.

### UI 작업
- 모든 UI 패널은 `Framework.UI.UIPanel`을 상속합니다 — `MonoBehaviour` 직접 상속 금지
- 패널 열기/닫기는 `UIManager.Instance.Open<T>()` / `Close()` / `ShowOverlay<T>()` 사용
- HUD처럼 항상 표시되는 패널은 `ShowOverlay<T>()`, 일반 패널은 `Open<T>()`
- `CanClose`, `CloseOnBack`, `DestroyOnClose`를 override하여 패널 동작 제어
- 초기화 데이터가 필요한 패널은 `IInitializable<TData>` 구현
- 패널 프리팹은 `Resources/UI/<ClassName>` 경로에 저장
- **버튼/토글 등 UI 이벤트는 Unity Inspector OnClick이 아닌 스크립트에서 `AddListener`로 연결** — Inspector 연결은 수정 시 추적이 어려워 유지보수가 힘듦

### 데이터 작업
- 게임 전용 데이터 로더는 `static class`로 작성, `InGameDataManager.Instance` 통해 호출
- `partial class InGameDataManager`는 프레임워크 namespace를 오염시키므로 사용 금지

### 입력 작업
- 모바일 입력은 `MobileInputManager`를 상속한 게임 전용 InputManager 사용

---

## 가이드라인 문서

- [코딩 스타일 규칙](Guidelines/CODING_STYLE.md) — 네이밍, 중괄호, Region 구조 등 상세 규칙
- [개인 코딩 스타일 요약](Guidelines/MY_CODING_STYLE.md) — 핵심 스타일 요약
- [프레임워크 구조](Guidelines/FRAMEWORK_STRUCTURE.md) — 디렉토리 구조, 레이어 아키텍처, 네임스페이스 규칙
- [개발 예정 모듈](Guidelines/PLANNED_MODULES.md) — 구현 예정 모듈 목록 및 진행 상황
- [커밋 스타일 규칙](Guidelines/COMMIT_STYLE.md) — 커밋 메시지 접두어 규칙
- [문서화 규칙](Guidelines/DOCS_STYLE.md) — 모듈 완료 기준, README 구조, 모듈 MD 템플릿
