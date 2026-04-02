# MathExtensions

게임 개발에서 자주 사용되는 수학 유틸리티 메서드 모음입니다.

---

## 개요

`Mathf`에 없는 게임용 수학 헬퍼를 정적 메서드로 제공합니다.

| 카테고리 | 메서드 |
|----------|--------|
| 범위 | `Remap`, `LoopIndex` |
| 비교 | `IsApproximately` |
| 반올림 | `RoundToDecimal`, `Snap` |
| 각도 | `NormalizeAngle` |

---

## 의존성

- 없음

---

## API 레퍼런스

### MathExtensions

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `Remap(value, fromMin, fromMax, toMin, toMax)` | `float` | 값을 한 범위에서 다른 범위로 재매핑 |
| `LoopIndex(index, length)` | `int` | 인덱스를 0~length 범위로 순환 (음수 포함) |
| `IsApproximately(a, b, tolerance)` | `bool` | 두 float의 근사 비교 |
| `RoundToDecimal(value, digits)` | `float` | 소수점 n자리 반올림 |
| `Snap(value, step)` | `float` | 지정 단위로 스냅 |
| `NormalizeAngle(angle)` | `float` | 각도를 -180~180으로 정규화 |

---

## 사용법

### Remap

```csharp
// HP(0~100)를 슬라이더 값(0~1)으로 변환
float sliderValue = MathExtensions.Remap(hp, 0f, 100f, 0f, 1f);

// 거리(0~50)를 볼륨(1~0)으로 변환
float volume = MathExtensions.Remap(distance, 0f, 50f, 1f, 0f);
```

### LoopIndex

```csharp
// 배열 순환 (음수도 안전하게 처리)
int next = MathExtensions.LoopIndex(currentIndex + 1, items.Length);
int prev = MathExtensions.LoopIndex(currentIndex - 1, items.Length);
```

### IsApproximately

```csharp
// float 비교 (== 대신 사용)
if (MathExtensions.IsApproximately(transform.position.x, targetX))
{
    // 도착 처리
}
```

### Snap

```csharp
// 0.5 단위로 스냅 (그리드 배치)
float snappedX = MathExtensions.Snap(rawX, 0.5f);
```

### NormalizeAngle

```csharp
// 270도 → -90도
float angle = MathExtensions.NormalizeAngle(270f); // -90
```

---

## 예시 (심화)

### 탭 UI 인덱스 순환

```csharp
public void NextTab()
{
    _currentTab = MathExtensions.LoopIndex(_currentTab + 1, _tabs.Length);
    UpdateTab();
}

public void PrevTab()
{
    _currentTab = MathExtensions.LoopIndex(_currentTab - 1, _tabs.Length);
    UpdateTab();
}
```

### 거리 기반 BGM 볼륨

```csharp
float distance = Vector3.Distance(player.position, source.position);
float volume = MathExtensions.Remap(distance, 0f, _maxDistance, 1f, 0f);
AudioManager.Instance.SetBGMVolume(volume);
```

---

## 주의사항

- `Remap`은 value가 fromMin~fromMax 범위를 벗어나도 클램핑하지 않습니다. 범위 제한이 필요하면 `Mathf.Clamp`를 추가로 사용하세요.
- `LoopIndex`는 length가 0이면 `DivideByZeroException`이 발생합니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-03 | 최초 작성 |
