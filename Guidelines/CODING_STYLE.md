# Coding Style Rules

Unity C# 프로젝트의 코딩 스타일 규칙을 정의합니다.

---

## 1. 네이밍 컨벤션 (Microsoft 표준)

| 대상 | 규칙 | 예시 |
|------|------|------|
| 클래스, 구조체, 인터페이스 | PascalCase | `PlayerController`, `IMovable` |
| public 필드 / 프로퍼티 / 메서드 | PascalCase | `MaxHealth`, `GetDamage()` |
| private / protected 필드 | `_camelCase` | `_currentHealth`, `_isAlive` |
| 지역 변수, 매개변수 | camelCase | `deltaTime`, `targetPosition` |
| 상수 (const) | PascalCase | `MaxSpeed`, `DefaultLayer` |
| 인터페이스 | `I` 접두사 + PascalCase | `IAttackable`, `IPoolable` |
| 추상 클래스 | `Base` 접미사 (선택) | `BaseCharacter`, `BaseState` |
| 제네릭 타입 매개변수 | `T` 또는 `T` + 의미 | `T`, `TState`, `TItem` |
| 이벤트 / Action / Func | PascalCase | `OnDead`, `OnHealthChanged` |

---

## 2. 중괄호 스타일 (Allman)

중괄호는 항상 새 줄에 작성합니다.

```csharp
// ✅ Good
if (condition)
{
    DoSomething();
}

public void Update()
{
    if (_isAlive)
    {
        Move();
    }
}

// ❌ Bad
if (condition) {
    DoSomething();
}
```

한 줄짜리 블록도 중괄호를 생략하지 않습니다.

```csharp
// ✅ Good
if (condition)
{
    return;
}

// ❌ Bad
if (condition) return;
```

---

## 3. 접근 제한자

- 접근 제한자는 항상 명시적으로 작성합니다.
- `public` 필드는 최소화하고, `[SerializeField] private`를 우선 사용합니다.
- `private`은 생략하지 않습니다.

```csharp
// ✅ Good
[SerializeField] private int _maxHealth;
private Rigidbody _rigidbody;
public int Level { get; private set; }

// ❌ Bad
int maxHealth;
Rigidbody rigidbody;
```

---

## 4. Region 구조

`#region`으로 코드 섹션을 구분합니다. 순서는 아래를 따릅니다.

```csharp
#region Fields
// 필드 선언
#endregion

#region Properties
// 프로퍼티
#endregion

#region Unity Lifecycle
// Awake, Start, Update, OnDestroy 등
#endregion

#region Public Methods
// 외부 공개 메서드
#endregion

#region Private Methods
// 내부 메서드
#endregion

#region Event Handlers
// 이벤트 콜백
#endregion
```

---

## 5. 주석

- 주석 언어는 상황에 따라 한국어/영어 혼용합니다.
- 복잡한 로직, 비직관적인 코드에 주석을 작성합니다.
- 자명한 코드에는 주석을 달지 않습니다.

```csharp
// ✅ Good - 비직관적인 로직 설명
// 음수 방향으로 이동하면 flipX 처리
_spriteRenderer.flipX = _velocity.x < 0;

// ❌ Bad - 자명한 코드에 주석
// 체력을 1 감소
_health -= 1;
```

XML 문서 주석은 public API에 작성합니다.

```csharp
/// <summary>
/// 데미지를 적용하고 사망 여부를 반환합니다.
/// </summary>
/// <param name="amount">적용할 데미지 양</param>
/// <returns>사망 시 true</returns>
public bool ApplyDamage(int amount)
```

---

## 6. 인터페이스 활용

컴포넌트 간 결합도를 낮추기 위해 인터페이스를 적극 사용합니다.

```csharp
// 의존성은 인터페이스로 주입
public class AttackSystem
{
    private IDamageable _target;

    public void Attack(IDamageable target)
    {
        target.TakeDamage(10);
    }
}
```

---

## 7. 기타 규칙

- 한 파일 = 한 클래스 (중첩 클래스 제외)
- 파일명 = 클래스명 (예: `PlayerController.cs`)
- `using` 정리: 미사용 네임스페이스 제거
- `var` 사용: 타입이 명확히 보이는 경우에만 허용
- 빈 줄: 메서드 사이에 한 줄, 논리 블록 사이에 한 줄
