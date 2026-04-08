# InputManager

New Input System 기반의 플랫폼별 입력 관리 모듈입니다.

---

## 개요

공통 기반 클래스인 `BaseInputManager`를 상속하여 플랫폼에 맞는 구현체를 씬에 배치합니다.

| 클래스 | 플랫폼 | 지원 입력 |
|--------|--------|-----------|
| `DesktopInputManager` | PC / 콘솔 | 키보드, 마우스, 게임패드 |
| `MobileInputManager` | 모바일 | 탭, 스와이프, 핀치 |

`BaseInputManager.Instance`로 공통 접근하며, 플랫폼별 메서드는 캐스팅하여 사용합니다.

---

## 왜 InputManager를 쓰는가

Unity New Input System을 각 컴포넌트에서 직접 사용할 수도 있지만, 아래 문제가 생깁니다.

**InputAction 참조가 흩어짐**
`[SerializeField] private InputAction _jumpAction` 같은 참조가 여러 컴포넌트에 분산되어, 키 바인딩 변경 시 모든 컴포넌트를 수정해야 합니다.

**플랫폼 분기가 게임 로직에 섞임**
`#if UNITY_ANDROID` 같은 플랫폼 분기나 터치/키보드 분기 코드가 게임 로직 곳곳에 들어가 가독성이 떨어집니다.

**InputManager를 쓰면**
- 입력 처리를 한 곳에서 관리하여 키 바인딩 변경 시 InputManager만 수정
- 게임 로직은 플랫폼을 몰라도 됨 — `GetButtonDown(action)` 하나로 통일
- 플랫폼 전환 시 씬의 InputManager 컴포넌트만 교체

---

## 의존성

- `Framework.Core.Singleton` — `PersistentMonoSingleton<T>` 상속
- `com.unity.inputsystem` — Unity New Input System 패키지

---

## API 레퍼런스

### 클래스

| 이름 | 설명 |
|------|------|
| `BaseInputManager` | 공통 기반 클래스 (PersistentMonoSingleton) |
| `DesktopInputManager` | PC/콘솔 입력 구현체 |
| `MobileInputManager` | 모바일 입력 구현체 |
| `MouseButton` | 마우스 버튼 열거형 (Left, Right, Middle) |

### BaseInputManager 공통 메서드

| 메서드 | 반환 타입 | 설명 |
|--------|-----------|------|
| `GetButtonDown(InputAction action)` | `bool` | Action이 이번 프레임에 눌렸는지 |
| `GetButton(InputAction action)` | `bool` | Action이 현재 눌려 있는지 |
| `GetButtonUp(InputAction action)` | `bool` | Action이 이번 프레임에 떼어졌는지 |
| `GetAxis(InputAction action)` | `Vector2` | Action의 Vector2 값 |

### DesktopInputManager

| 메서드 / 프로퍼티 | 반환 타입 | 설명 |
|------------------|-----------|------|
| `MousePosition` | `Vector2` | 현재 마우스 위치 (스크린 좌표) |
| `MouseDelta` | `Vector2` | 마우스 이동 델타 |
| `ScrollDelta` | `Vector2` | 마우스 스크롤 델타 |
| `GetMouseButtonDown(MouseButton button)` | `bool` | 마우스 버튼 눌림 (기본값: Left) |
| `GetMouseButton(MouseButton button)` | `bool` | 마우스 버튼 유지 (기본값: Left) |
| `GetMouseButtonUp(MouseButton button)` | `bool` | 마우스 버튼 뗌 (기본값: Left) |
| `IsGamepadConnected()` | `bool` | 게임패드 연결 여부 |
| `GetGamepadLeftStick()` | `Vector2` | 게임패드 왼쪽 스틱 |
| `GetGamepadRightStick()` | `Vector2` | 게임패드 오른쪽 스틱 |

### MobileInputManager

| 메서드 / 프로퍼티 | 반환 타입 | 설명 |
|------------------|-----------|------|
| `TouchCount` | `int` | 현재 활성 터치 개수 |
| `IsPinching` | `bool` | 핀치 제스처 중 여부 (터치 2개) |
| `IsTouching()` | `bool` | 터치 중 여부 |
| `GetTap()` | `bool` | 이번 프레임 탭 발생 여부 |
| `GetSwipe(out Vector2 direction)` | `bool` | 스와이프 발생 여부 및 방향 |
| `GetPinchDelta()` | `float` | 핀치 델타 (양수: 확대, 음수: 축소) |
| `GetTouchPosition()` | `Vector2` | 첫 번째 터치 위치 |

---

## 사용법

### DesktopInputManager

```csharp
// 씬에 DesktopInputManager 컴포넌트를 가진 GameObject를 배치하거나
// 코드에서 Instance 접근 시 자동 생성됩니다.

var input = BaseInputManager.Instance as DesktopInputManager;

// 마우스
if (input.GetMouseButtonDown())                          // 좌클릭
if (input.GetMouseButtonDown(MouseButton.Right))         // 우클릭
Vector2 pos = input.MousePosition;

// InputAction 기반 (키보드, 게임패드 공통)
if (input.GetButtonDown(jumpAction))
Vector2 move = input.GetAxis(moveAction);
```

### MobileInputManager

```csharp
var input = BaseInputManager.Instance as MobileInputManager;

// 탭
if (input.GetTap())
{
    Debug.Log("탭!");
}

// 스와이프
if (input.GetSwipe(out Vector2 dir))
{
    Debug.Log($"스와이프 방향: {dir}");
}

// 핀치
float pinch = input.GetPinchDelta();
camera.orthographicSize -= pinch * 0.01f;
```

---

## 주의사항

- `DesktopInputManager` 또는 `MobileInputManager` 중 하나만 씬에 배치하세요. 둘 다 있으면 나중에 생성된 것이 파괴됩니다.
- `MobileInputManager`는 내부적으로 `EnhancedTouchSupport`를 활성화합니다. 다른 곳에서 중복 활성화하지 않도록 주의하세요.
- `GetSwipe`, `GetTap`의 임계값은 Inspector에서 `_swipeThreshold`, `_tapTimeThreshold`로 조절할 수 있습니다.
- `BaseInputManager.Instance`는 `BaseInputManager` 타입을 반환합니다. 플랫폼별 메서드 사용 시 캐스팅이 필요합니다.

---

## 뒤로가기(Back Key) 처리

UIManager / CommonPopupManager는 입력 코드를 직접 갖지 않습니다.
뒤로가기 처리는 반드시 InputManager를 상속받은 곳에서 수행하세요.

- `DesktopInputManager` — Escape 키 감지 시 `HandleBack()` 호출
- `MobileInputManager` — Android Back 버튼(New Input System에서 Escape로 매핑) 감지 시 `HandleBack()` 호출
- `BaseInputManager.HandleBack()`은 기본 구현이 비어 있으므로 게임에서 상속받아 override하세요.

```csharp
// 게임 코드 예시 (프레임워크 외부)
public class GameInputManager : MobileInputManager
{
    protected override void HandleBack()
    {
        // 팝업 먼저, 없으면 UIManager
        if (CommonPopupManager.Instance.HandleBack()) return;
        UIManager.Instance.HandleBack();
    }
}
```

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.1.0 | 2026-04-08 | HandleBack() 추가 (Escape / Android Back 감지, 상속 override 방식) |
| 1.0.0 | 2026-04-01 | 최초 작성 |
