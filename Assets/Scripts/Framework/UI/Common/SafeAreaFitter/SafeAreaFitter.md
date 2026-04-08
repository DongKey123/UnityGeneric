# SafeAreaFitter

디바이스의 Safe Area에 맞게 RectTransform을 자동으로 조정하는 컴포넌트입니다.

---

## 개요

노치, 펀치홀, 홈 인디케이터 등으로 인해 UI가 가려지는 것을 방지합니다.
`Screen.safeArea`를 Canvas 기준으로 정규화하여 RectTransform의 `anchorMin/anchorMax`에 적용합니다.
디바이스 회전 시 자동으로 재계산됩니다.

---

## 의존성

없음

---

## API 레퍼런스

### SafeAreaFitter 프로퍼티 (Inspector)

| 필드 | 타입 | 기본값 | 설명 |
|------|------|--------|------|
| `Apply Top` | `bool` | true | 상단 Safe Area 적용 여부 |
| `Apply Bottom` | `bool` | true | 하단 Safe Area 적용 여부 (홈 인디케이터) |
| `Apply Left` | `bool` | true | 좌측 Safe Area 적용 여부 |
| `Apply Right` | `bool` | true | 우측 Safe Area 적용 여부 |

---

## 사용법

### 기본 사용

UIPanel의 루트 GameObject에 컴포넌트를 추가합니다.

```
InventoryPanel (Canvas)
└── SafeArea (RectTransform) ← SafeAreaFitter 추가
    └── ... UI 요소들
```

UIPanel 루트에 직접 붙이지 않고, **자식 GameObject**에 붙여서 Safe Area 영역을 컨테이너로 사용하는 것을 권장합니다.

### 방향별 선택 적용

BottomSheet처럼 하단만 Safe Area가 필요한 경우:

```
Apply Top    ☐
Apply Bottom ☑
Apply Left   ☐
Apply Right  ☐
```

---

## Details

### Screen.safeArea 정규화 방식

```
anchorMin = safeArea.position / screenSize
anchorMax = (safeArea.position + safeArea.size) / screenSize
```

Canvas의 픽셀 좌표계를 0~1 범위로 정규화하여 해상도와 무관하게 동작합니다.

### 디바이스 회전 감지

`OnRectTransformDimensionsChange()`를 사용하므로 Update 오버헤드가 없습니다.
화면 크기나 방향이 바뀔 때만 호출되어 자동으로 재적용됩니다.

---

## 주의사항

- `Screen.safeArea`는 에디터에서 항상 전체 화면을 반환합니다. 실제 기기 또는 Device Simulator로 테스트하세요.
- RectTransform의 `anchorMin/anchorMax`를 직접 수정하므로, 같은 오브젝트에서 앵커를 코드로 제어하면 충돌이 발생합니다.

---

## 변경 이력

| 버전 | 날짜 | 내용 |
|------|------|------|
| 1.0.0 | 2026-04-08 | 최초 작성 |
