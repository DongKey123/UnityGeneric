# 미니맵 시스템 설계

> 방식: 마커(dot) 표시 전용 — RenderTexture 없음
> 기준 위치: 좌상단 고정 HUD

---

## 구현 계획

### 생성 파일

| 파일 | 역할 |
|---|---|
| `Scripts/Defines/MinimapConsts.cs` | `DisplayRadius = 50f` 등 상수 |
| `Scripts/UI/Minimap/MinimapMarker.cs` | 월드 오브젝트에 붙는 마커 컴포넌트 |
| `Scripts/UI/Minimap/MinimapPanel.cs` | UIPanel 상속, 마커 위치 계산 및 dot 표시 |

### 수정 파일

| 파일 | 변경 내용 |
|---|---|
| `Scripts/Core/SurvivalEntry.cs` | `UIManager.ShowOverlay<MinimapPanel>()` 추가 |

---

## 각 파일 상세

### MinimapConsts.cs
```csharp
public static class MinimapConsts
{
    public const float DisplayRadius = 50f;  // 월드 기준 표시 반경 (m)
}
```

### MinimapMarker.cs
- `MarkerType` 열거형: `Player`, `Enemy`, `Resource`
- `OnEnable` → 정적 리스트에 자신 등록
- `OnDisable` → 리스트에서 해제
- `WorldPosition` 프로퍼티 제공

### MinimapPanel.cs
- `UIPanel` 상속, `ShowOverlay` 방식으로 열기
- `CanClose = false`, `CloseOnBack = false`
- Inspector 필드:
  - `_playerDot`, `_enemyDotPrefab`, `_resourceDotPrefab` (Image)
  - `_markerContainer` (RectTransform — 마커 dot들의 부모)
  - `_minimapRadius` (float — UI 반경 px, 기본 78)
- `Update()` 매 프레임:
  1. 등록된 마커 목록 순회
  2. 플레이어 기준 상대 좌표 계산 (xz 평면)
  3. `DisplayRadius` 밖이면 원 가장자리로 클램프
  4. dot Image 위치 갱신

**위치 계산식:**
```
relPos   = (marker.WorldPos - player.WorldPos).xz
uiPos    = relPos / DisplayRadius * _minimapRadius
// 클램프
if (uiPos.magnitude > _minimapRadius)
    uiPos = uiPos.normalized * _minimapRadius
```

---

## 프리팹 구조

```
MinimapPanel (Canvas, UIPanel)
└── MinimapRoot (RectTransform 156×156, 좌상단 anchor)
    ├── MaskArea (Image: circle, Mask 컴포넌트)
    │   ├── Background (Image: minimap_bg.png)
    │   └── MarkerContainer (RectTransform) ← dot들 여기에
    └── Frame (Image: minimap_frame.png) ← 마스크 밖, 맨 위
```

---

## Unity 에디터 작업 (코드 완성 후)

- [ ] MinimapPanel.prefab 제작 (원형 마스크 + MarkerContainer + Frame)
- [ ] dot 스프라이트 연결 (`dot_player`, `dot_enemy`, `dot_resource`)
- [ ] Player GameObject에 `MinimapMarker` (Type: Player) 추가
- [ ] Enemy_Zombie / Enemy_Wolf 프리팹에 `MinimapMarker` (Type: Enemy) 추가
- [ ] Resource_Tree / Resource_Rock 프리팹에 `MinimapMarker` (Type: Resource) 추가

---

## 마커 스프라이트

| 타입 | 스프라이트 | 색상 |
|---|---|---|
| Player | `dot_player.png` | 흰색 |
| Enemy | `dot_enemy.png` | 빨강 |
| Resource | `dot_resource.png` | 녹색 |

경로: `Assets/Game_Survival/Art/UI/Main/`
