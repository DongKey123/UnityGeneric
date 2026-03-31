# Claude Instructions

이 프로젝트는 Unity용 재사용 가능한 제네릭 프레임워크입니다.
아래 가이드라인 문서를 반드시 참고하여 코드를 작성하세요.

## 모듈 완료 시 필수 작업

모듈 구현이 완료될 때마다 아래 항목을 반드시 업데이트하세요.

1. 해당 모듈 폴더에 `<ModuleName>.md` 작성 ([문서 템플릿 참고](Guidelines/DOCS_STYLE.md))
2. 해당 모듈 폴더에 `<ModuleName>.unitypackage` 추출
3. `README.md` 모듈 목록에서 해당 모듈 상태를 ✅ 로 변경 — README의 모든 섹션(Core/Patterns/UI/Utils)에 해당 모듈이 누락 없이 등록되어 있는지 함께 확인
4. `README.md` 변경 이력에 버전, 날짜, 내용 추가
5. 작업된 모든 파일의 XML 주석 검토 — `&lt;` / `&gt;` 엔티티 노출, `<param>` / `<returns>` 누락 여부 확인 후 수정

---

## 가이드라인 문서

- [코딩 스타일 규칙](Guidelines/CODING_STYLE.md) — 네이밍, 중괄호, Region 구조 등 상세 규칙
- [개인 코딩 스타일 요약](Guidelines/MY_CODING_STYLE.md) — 핵심 스타일 요약
- [프레임워크 구조](Guidelines/FRAMEWORK_STRUCTURE.md) — 디렉토리 구조, 레이어 아키텍처, 네임스페이스 규칙
- [개발 예정 모듈](Guidelines/PLANNED_MODULES.md) — 구현 예정 모듈 목록 및 진행 상황
- [커밋 스타일 규칙](Guidelines/COMMIT_STYLE.md) — 커밋 메시지 접두어 규칙
- [문서화 규칙](Guidelines/DOCS_STYLE.md) — 모듈 완료 기준, README 구조, 모듈 MD 템플릿
