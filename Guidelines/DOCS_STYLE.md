# Docs Style Rules

포트폴리오 및 모듈 문서화 규칙을 정의합니다.

---

## 모듈 완료 기준

아래 두 가지가 모두 존재해야 해당 모듈이 **완료** 상태입니다.

1. 모듈 폴더 내 `<ModuleName>.md` 문서 파일
2. 모듈 폴더 내 `<ModuleName>.unitypackage` 익스포트 파일

---

## 디렉토리 구조 예시

```
Assets/Framework/Core/Singleton/
├── Singleton.cs
├── Singleton.md              ← 모듈 문서 (필수)
└── Singleton.unitypackage    ← 완료 표시 (필수)
```

---

## 루트 README.md 구조

프로젝트 루트의 `README.md`는 프로젝트 소개와 모듈 목록 링크를 포함합니다.
모듈 목록 상세는 [PLANNED_MODULES.md](PLANNED_MODULES.md)를 참조합니다.

```markdown
# Unity Generic Framework

Unity 재사용 프레임워크입니다.

## 모듈 목록

전체 모듈 목록 및 진행 상황은 [PLANNED_MODULES.md](Guidelines/PLANNED_MODULES.md)를 참고하세요.

## 상태 아이콘
- ✅ 완료 (md + unitypackage 존재)
- 🚧 진행 중
- ⬜ 미시작
```

---

## 모듈 MD 파일 구조

각 모듈 폴더 내 `<ModuleName>.md`는 아래 구조를 따릅니다.

```markdown
# <ModuleName>

한 줄 설명.

---

## 개요

모듈의 목적과 해결하는 문제를 설명합니다.

---

## 의존성

- 의존하는 다른 Framework 모듈 또는 Unity 패키지를 명시합니다.
- 없으면 `없음` 으로 작성합니다.

---

## API 레퍼런스

### 클래스 / 인터페이스

| 이름 | 설명 |
|------|------|
| `ClassName` | 설명 |

### 주요 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `MethodName(param)` | `void` | 설명 |

---

## 사용법

기본 사용 예시를 작성합니다.

​```csharp
// 예시 코드
​```

---

## 예시 (심화)

실제 게임 로직에 적용하는 예시를 작성합니다.

​```csharp
// 심화 예시 코드
​```

---

## 주의사항

- 사용 시 알아야 할 제약, 주의할 점을 작성합니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | YYYY-MM-DD | 최초 작성 |
```
