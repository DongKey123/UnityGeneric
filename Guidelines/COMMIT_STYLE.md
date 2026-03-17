# Commit Style Rules

커밋 메시지 작성 규칙을 정의합니다.

---

## 접두어 규칙

| 접두어 | 용도 | 예시 |
|--------|------|------|
| `[Add]` | 새로운 기능 추가 | `[Add] Singleton 기본 구현` |
| `[Del]` | 기능 또는 파일 삭제 | `[Del] 미사용 ObjectPool 제거` |
| `[Mod]` | 기존 기능 변경 | `[Mod] StateMachine 전환 로직 수정` |
| `[Fix]` | 버그 수정 | `[Fix] ObjectPool 반환 시 null 예외 수정` |
| `[Refactor]` | 기능 변경 없이 코드 구조 개선 | `[Refactor] EventBus 내부 구조 정리` |
| `[Docs]` | 문서 작업 | `[Docs] Singleton 사용법 주석 추가` |
| `[Chore]` | 빌드 설정, 패키지 등 기타 작업 | `[Chore] .gitignore 업데이트` |

---

## 작성 형식

```
[접두어] 변경 내용 요약
```

---

## 예시

```
[Add] EventBus 글로벌 이벤트 시스템 구현
[Mod] Singleton 씬 전환 시 파괴 옵션 추가
[Del] 레거시 Timer 유틸리티 제거
[Fix] ObjectPool 반환 시 null 예외 수정
[Refactor] StateMachine 전환 로직 구조 개선
[Docs] 프레임워크 구조 문서 업데이트
[Chore] .gitignore Unity 규칙 추가
```
