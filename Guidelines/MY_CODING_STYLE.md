# My Coding Style

개인 코딩 스타일 및 선호 패턴을 정의합니다.

---

## 네이밍

- **Microsoft 표준**을 따릅니다.
- `public` 멤버: `PascalCase`
- `private` / `protected` 필드: `_camelCase` (언더스코어 접두사)
- 지역 변수 / 매개변수: `camelCase`

---

## 중괄호

- **Allman 스타일** — 중괄호는 항상 새 줄에 작성합니다.
- 한 줄짜리 블록도 중괄호를 생략하지 않습니다.

---

## 접근 제한자

- `public` 필드 대신 `[SerializeField] private`를 선호합니다.
- Inspector에 노출할 값은 `[SerializeField]`로 관리합니다.

```csharp
[SerializeField] private int _maxHealth;
[SerializeField] private float _moveSpeed;
```

---

## Region

`#region`으로 코드를 논리적으로 구분합니다.

```csharp
#region Fields
#endregion

#region Properties
#endregion

#region Unity Lifecycle
#endregion

#region Public Methods
#endregion

#region Private Methods
#endregion
```

---

## 인터페이스

- 컴포넌트 간 결합도를 낮추기 위해 인터페이스를 적극 활용합니다.
- 기능 단위로 인터페이스를 분리합니다 (ISP 원칙).

```csharp
public interface IDamageable
{
    void TakeDamage(int amount);
}

public interface IHealable
{
    void Heal(int amount);
}
```

---

## 주석

- 한국어 / 영어 혼용합니다.
- 비직관적인 로직에만 주석을 작성합니다.
- public API에는 XML 문서 주석을 작성합니다.

---

## 기타 선호

- 한 파일 = 한 클래스
- `var`는 타입이 명확히 보이는 경우에만 사용
- 이른 반환(early return)으로 중첩 줄이기
