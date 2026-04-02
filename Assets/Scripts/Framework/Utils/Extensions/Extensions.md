# Extensions

Unity 타입에 대한 확장 메서드 모음입니다.

---

## 개요

| 클래스 | 대상 타입 | 설명 |
|--------|-----------|------|
| `TransformExtensions` | `Transform` | 개별 축 Position 설정, 초기화, 2D LookAt, 자식 삭제 |
| `VectorExtensions` | `Vector2`, `Vector3` | 특정 축 교체, 타입 변환, 랜덤 생성 |
| `ColorExtensions` | `Color` | 특정 채널 교체, Hex 변환 |

---

## 의존성

- 없음

---

## API 레퍼런스

### TransformExtensions

| 메서드 | 설명 |
|--------|------|
| `SetPositionX/Y/Z(float)` | World Position 개별 축 설정 |
| `SetLocalPositionX/Y/Z(float)` | Local Position 개별 축 설정 |
| `ResetLocal()` | localPosition/Rotation/Scale 초기화 |
| `LookAt2D(Vector2 target)` | 2D 환경에서 타겟 방향으로 Z축 회전 |
| `DestroyAllChildren()` | 모든 자식 오브젝트 제거 |

### VectorExtensions

| 메서드 | 설명 |
|--------|------|
| `WithX/Y/Z(float)` | 특정 축만 교체한 새 Vector3 반환 |
| `WithX/Y(float)` | 특정 축만 교체한 새 Vector2 반환 |
| `ToVector2()` | Vector3 → Vector2 (XY 성분) |
| `ToVector3(float z)` | Vector2 → Vector3 (Z 기본값 0) |
| `RandomRange(Vector3 min, Vector3 max)` | 두 벡터 사이의 랜덤 Vector3 반환 |

### ColorExtensions

| 메서드 | 설명 |
|--------|------|
| `WithAlpha(float)` | Alpha만 교체한 새 Color 반환 |
| `WithR/G/B(float)` | 특정 채널만 교체한 새 Color 반환 |
| `ToHex()` | Color → "#RRGGBB" Hex 문자열 |
| `FromHex(string)` | "#RRGGBB" Hex 문자열 → Color (static) |

---

## 사용법

### TransformExtensions

```csharp
// Y 위치만 변경
transform.SetPositionY(5f);

// Local 초기화
transform.ResetLocal();

// 2D에서 마우스 방향 바라보기
Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
transform.LookAt2D(mouseWorld);

// 모든 자식 제거
transform.DestroyAllChildren();
```

### VectorExtensions

```csharp
// Y만 바꾼 새 Vector3
Vector3 newPos = transform.position.WithY(0f);

// Vector3 → Vector2
Vector2 pos2D = transform.position.ToVector2();

// Vector2 → Vector3
Vector3 pos3D = pos2D.ToVector3(z: 1f);

// 랜덤 스폰 위치
Vector3 spawnPos = VectorExtensions.RandomRange(Vector3.zero, new Vector3(10f, 0f, 10f));
```

### ColorExtensions

```csharp
// 알파만 변경 (반투명)
_image.color = _image.color.WithAlpha(0.5f);

// Hex → Color
Color red = ColorExtensions.FromHex("#FF0000");

// Color → Hex
string hex = Color.red.ToHex(); // "#FF0000"
```

---

## 예시 (심화)

### UI 페이드 아웃

```csharp
private IEnumerator FadeOut(Image image, float duration)
{
    float elapsed = 0f;
    Color original = image.color;

    while (elapsed < duration)
    {
        elapsed += Time.deltaTime;
        image.color = original.WithAlpha(1f - elapsed / duration);
        yield return null;
    }

    image.color = original.WithAlpha(0f);
}
```

### 2D 투사체 방향 설정

```csharp
void Fire(Vector2 target)
{
    GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity);
    bullet.transform.LookAt2D(target);
    bullet.GetComponent<Rigidbody2D>().velocity =
        (Vector3)(target - (Vector2)transform.position).normalized * _speed;
}
```

---

## 주의사항

- `SetPositionX/Y/Z`는 World Position을 변경합니다. Local을 변경하려면 `SetLocalPositionX/Y/Z`를 사용하세요.
- `DestroyAllChildren`은 `Object.Destroy`를 사용하므로 실제 제거는 다음 프레임에 이루어집니다.
- `FromHex`는 static 메서드이므로 `ColorExtensions.FromHex("#FF0000")` 형태로 호출합니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-03 | 최초 작성 (TransformExtensions, VectorExtensions, ColorExtensions) |
