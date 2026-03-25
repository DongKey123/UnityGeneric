# ObjectPool

MonoBehaviour 컴포넌트를 재사용하는 제네릭 오브젝트 풀 모듈입니다.

---

## 개요

게임에서 총알, 이펙트, 적 등 자주 생성/파괴되는 오브젝트를 매번 `Instantiate`/`Destroy`하면 GC가 자주 발생해 성능 저하로 이어집니다.
오브젝트 풀은 오브젝트를 미리 만들어두고 비활성화 상태로 보관하다가 필요할 때 꺼내 쓰고, 다 쓰면 파괴하지 않고 다시 반환하는 패턴입니다.

| 클래스 | 역할 |
|--------|------|
| `IPoolable` | 풀링 대상 컴포넌트가 구현해야 하는 인터페이스 |
| `ObjectPool<T>` | 단일 타입 오브젝트를 관리하는 풀 |
| `ObjectPoolManager` | 여러 풀을 이름(key) 기반으로 중앙 관리하는 싱글톤 매니저 |

<details>
<summary>기본 개념 — 오브젝트 풀이란?</summary>

`Instantiate`와 `Destroy`는 메모리 할당/해제를 동반하며, 이 과정에서 GC(Garbage Collector)가 동작해 프레임 드랍을 유발할 수 있습니다.

오브젝트 풀은 이를 해결하기 위해:
1. 미리 일정 수의 오브젝트를 생성해 `Queue`에 보관
2. 필요할 때 `Queue`에서 꺼내 활성화 (`Get`)
3. 사용 후 파괴하지 않고 비활성화해서 반환 (`Release`)

이 방식으로 `Instantiate`/`Destroy` 호출을 최소화해 GC 부하를 줄입니다.

</details>

---

## 의존성

- `Framework.Core.Singleton` (ObjectPoolManager가 PersistentMonoSingleton 사용)

---

## API 레퍼런스

### 클래스 / 인터페이스

| 이름 | 설명 |
|------|------|
| `IPoolable` | `OnSpawn()`, `OnDespawn()` 콜백을 정의하는 인터페이스 |
| `ObjectPool<T>` | `IPoolable`을 구현한 MonoBehaviour를 풀링. 독립적으로 사용 가능 |
| `ObjectPoolManager` | 여러 `ObjectPool<T>`을 key로 관리하는 PersistentMonoSingleton |

### 주요 메서드

#### IPoolable

| 메서드 | 설명 |
|--------|------|
| `OnSpawn()` | `Get()` 시 호출. 초기화 로직 작성 |
| `OnDespawn()` | `Release()` 시 호출. 정리 로직 작성 |

#### ObjectPool\<T\>

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Get()` | `T` | 풀에서 오브젝트를 꺼냄. 비어있으면 자동 생성 |
| `Release(T item)` | `void` | 오브젝트를 풀로 반환 |
| `Clear()` | `void` | 대기 중인 오브젝트 모두 파괴 |
| `CountInactive` | `int` | 현재 대기 중인 오브젝트 수 |

#### ObjectPoolManager

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Register<T>(key, prefab, initialSize)` | `void` | 풀 등록. 이미 등록된 key면 무시 |
| `Get<T>(key)` | `T` | 등록된 풀에서 오브젝트를 꺼냄 |
| `Release<T>(key, item)` | `void` | 오브젝트를 풀로 반환 |
| `Remove(key)` | `void` | 특정 풀 제거 및 대기 오브젝트 파괴 |
| `ClearAll()` | `void` | 모든 풀 제거 및 대기 오브젝트 파괴 |

---

## 사용법

**1. 풀링할 컴포넌트에 IPoolable 구현**

```csharp
public class Bullet : MonoBehaviour, IPoolable
{
    public void OnSpawn()
    {
        // Get() 시 초기화
        _velocity = Vector3.forward * 10f;
    }

    public void OnDespawn()
    {
        // Release() 시 정리
        _velocity = Vector3.zero;
    }
}
```

**2-A. ObjectPoolManager 사용 (권장)**

```csharp
// 등록 (보통 초기화 시점에)
ObjectPoolManager.Instance.Register("PlayerBullet", bulletPrefab, 30);
ObjectPoolManager.Instance.Register("EnemyBullet", bulletPrefab, 100);

// 꺼내기
Bullet bullet = ObjectPoolManager.Instance.Get<Bullet>("PlayerBullet");

// 반환
ObjectPoolManager.Instance.Release("PlayerBullet", bullet);
```

**2-B. ObjectPool 직접 사용**

```csharp
// 풀 생성
var pool = new ObjectPool<Bullet>(bulletPrefab, initialSize: 20);

// 꺼내기
Bullet bullet = pool.Get();

// 반환
pool.Release(bullet);

// 풀 정리
pool.Clear();
```

---

## 예시 (심화)

```csharp
// 총알 발사 시 풀에서 꺼내고, 일정 시간 후 반환
public class Gun : MonoBehaviour
{
    private const string BulletKey = "PlayerBullet";

    [SerializeField] private Bullet _bulletPrefab;

    private void Start()
    {
        ObjectPoolManager.Instance.Register(BulletKey, _bulletPrefab, 30);
    }

    public void Fire(Vector3 position, Vector3 direction)
    {
        Bullet bullet = ObjectPoolManager.Instance.Get<Bullet>(BulletKey);
        bullet.transform.position = position;
        bullet.transform.forward = direction;
    }
}

// 총알이 목표에 닿으면 반환
public class Bullet : MonoBehaviour, IPoolable
{
    private const string BulletKey = "PlayerBullet";

    public void OnSpawn() { }
    public void OnDespawn() { }

    private void OnTriggerEnter(Collider other)
    {
        ObjectPoolManager.Instance.Release(BulletKey, this);
    }
}
```

---

## 주의사항

- `IPoolable`을 구현하지 않은 컴포넌트는 풀링할 수 없습니다.
- `Get()`으로 꺼낸 오브젝트는 반드시 `Release()`로 반환해야 합니다. 직접 `Destroy`하면 풀이 해당 오브젝트를 추적하지 못합니다.
- `ObjectPoolManager`는 씬에 배치하지 않아도 `Instance` 접근 시 자동 생성됩니다.
- `Clear()` / `ClearAll()`은 대기 중인 오브젝트만 파괴합니다. 사용 중인 오브젝트는 호출자가 직접 처리해야 합니다.
- 같은 프리팹이라도 다른 key로 등록하면 독립적인 풀로 관리됩니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-03-25 | 최초 작성 |
