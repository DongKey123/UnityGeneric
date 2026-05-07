# Last Day Survival — HUD 01 Wasteland Protocol
## Unity Integration Guide

모바일 가로 (Landscape) 서바이벌 게임 HUD 에셋 패키지 — Last Day on Earth 스타일

---

## 📁 폴더 구조

```
unity-hud-export/
├── README.md                          ← 이 파일
├── Reference/
│   ├── hud_01_clean.png              ← 완성 시안 (참고용)
│   └── hud_01_annotated_guide.png    ← 번호 표시된 가이드
└── Sprites/                          ← Unity Image/RawImage용 PNG
    ├── icon_*.png                     아이콘 (흰색, Color tint 가능)
    ├── panel_*.png                    패널 배경 (9-slice 권장)
    ├── quickslot_*.png                퀵슬롯 프레임
    ├── joystick_*.png                 조이스틱
    ├── btn_*.png                      액션 버튼 (공격/채집)
    ├── hpbar_*.png / durabar_*.png    체력/내구도 바
    ├── weightbar_*.png / cycle_bar    무게 / 낮밤 사이클
    ├── minimap_*.png                  미니맵 프레임/바닥
    ├── dot_*.png                      미니맵 마커
    └── red_dot.png                    알림 빨간 점
```

---

## 🎨 디자인 토큰

| 항목 | 값 |
|---|---|
| Primary (Amber) | `#D59450` |
| Danger (Red) | `#C84A37` |
| Success (Green) | `#7BA84D` |
| BG Dark | `#0D1117` |
| Panel BG | `rgba(8,12,18,0.85)` |
| Text Light | `#E8EAF0` |
| Text Dim | `rgba(220,225,235,0.5)` |

**Fonts (Unity TextMeshPro 권장)**
- Display: **Oswald** (Bold/SemiBold) — 라벨, 지역명
- Mono: **Share Tech Mono** — 숫자, 좌표, 시간

---

## 📐 캔버스 설정 (Canvas)

```
Canvas
├── Render Mode: Screen Space - Overlay
├── Reference Resolution: 1920 × 900 (16:9 가로)
├── Match: Width or Height (Match: 0.5)
└── UI Scale Mode: Scale With Screen Size
```

기준 시안은 **812 × 375** (1x). Unity에서는 1920×900 또는 2436×1125로 작업 권장.
**모든 PNG는 2x 해상도로 추출**되어 있어 고해상도 디바이스에서도 선명함.

---

## 🧩 컴포넌트별 가이드 (참고: `hud_01_annotated_guide.png`)

### 1️⃣ 미니맵 (좌상단)
- **Position**: TopLeft, anchor (0,1), padding 10px
- **Size**: 78 × 78 (1x) / 156 × 156 (2x)
- **구조**:
  ```
  MiniMapRoot (RectTransform 78×78)
  ├── Mask (CircleMask)
  │   └── RawImage  ← RenderTexture from Camera
  └── Frame (Image: minimap_frame.png) ← 위에 오버레이
  ```
- **Texture**: 별도 카메라 + RenderTexture 사용. `minimap_bg.png`는 RenderTexture 없을 때 placeholder.
- **마커**: `dot_player.png` (흰색), `dot_enemy.png` (빨강), `dot_resource.png` (녹색)

### 2️⃣ 지역명 (Text)
- **Position**: 미니맵 우측, 상단 정렬
- **Components**: TextMeshPro Text
  - Label: "◈ LOCATION" — Share Tech Mono, 8pt, color `#D59450`
  - Value: "FOREST A" — Oswald Bold, 14pt, color `#E8EAF0`, letter-spacing 0.08em
  - Sub: "SECTOR 04 · SAFE" — Share Tech Mono, 8pt, color `rgba(200,210,220,0.4)`

### 3️⃣ 낮/밤 시계 (상단 중앙)
- **Position**: TopCenter, anchor (0.5, 1), y offset 16px
- **Background**: `panel_diagonal.png` (대각 클립)
- **구조**:
  ```
  ├── Icon (icon_sun.png, 13×13)
  ├── ProgressBar (Image: cycle_bar.png, Filled mode Horizontal)
  │   └── Indicator (1×7 흰색 막대, slider knob)
  └── Text "D 14 · 14:32" (Share Tech Mono, 9pt)
  ```
- **로직**: `fillAmount = (currentTimeOfDay / dayLengthSeconds)`

### 4️⃣ 재화 (우상단)
- **두 개의 작은 패널** — `panel_default.png`을 9-slice로 사용
- 각 패널: `icon_coin.png` 또는 `icon_gem.png` + TextMeshPro 숫자
- **카운팅 애니메이션**: DOTween `DOCounter()` 권장

### 5️⃣ 메뉴 버튼 (우상단)
- 30×30 정사각 버튼 3개: `panel_default.png` 배경 + 흰색 아이콘
  - 우편: `icon_mail.png` (+ `red_dot.png` 우상단 8×8)
  - 상점: `icon_shop.png`
  - 설정: `icon_settings.png`
- **Hover/Press**: ColorTint amber 활성화

### 6️⃣ HP 바 (캐릭터 머리 위 — World Space)
- **별도 World Space Canvas** 또는 Billboard
- **구조**: `hpbar_bg.png` (검은 BG) + `hpbar_fill.png` (빨간 채움, Filled Image)
- **Width**: 46px, follow 캐릭터 위치

### 7️⃣ 조이스틱 (좌하단)
- **Position**: BottomLeft, anchor (0,0), padding 18px
- **Size**: 92 × 92
- **구조**:
  ```
  JoystickRoot (Image: joystick_base.png, 92×92)
  └── Knob (Image: joystick_knob.png, 약 38×38)
  ```
- **로직**: 표준 Unity VirtualJoystick 컴포넌트 활용. `OnDrag`로 knob 이동 → 최대 반경 27px

### 8️⃣ 퀵슬롯 ×3 (하단 중앙)
- **Size**: 46 × 46 each, 5px gap
- **구조**:
  ```
  QuickSlot
  ├── Frame (Image: quickslot_default.png 또는 quickslot_selected.png)
  ├── ItemIcon (Image: 무기/도구 아이콘, 흰색 → ColorTint)
  ├── DurabilityBar (하단 2px)
  │   ├── BG: durabar_bg.png
  │   └── Fill: durabar_fill_green/yellow/red.png (FilledImage)
  └── QuantityText (TMP, 우하단, 9pt mono)
  ```
- **장착 시**: 프레임을 `quickslot_selected.png`로 swap (amber 2px border)
- **내구도 색상**: 50%↑ green / 25-50% yellow / <25% red

### 9️⃣ 무게 표시
- **Position**: 퀵슬롯 아래 중앙
- **구조**: `panel_default.png` 배경 + `icon_backpack.png` + `weightbar_bg/fill` + Text
- **경고색**: 90% ↑ 주황(`#E8A850`), 100%+ 빨강(`#C84A37`)

### 🔟 액션 바 (하단 중앙)
- 4개 50×50 버튼 가로 배치, 5px gap
- **각 버튼**: `panel_default.png` + 아이콘 + 라벨 텍스트
  - INV → `icon_backpack.png`
  - CRAFT → `icon_hammer.png`
  - BUILD → `icon_build.png`
  - MAP → `icon_map.png`

### 1️⃣1️⃣ 채집 버튼 (컨텍스트 — 우하단)
- **Size**: 54 × 54, 원형 (`btn_harvest.png`)
- **아이콘**: `icon_axe.png` (녹색 ColorTint `#B8DD80`)
- **라벨**: "HARVEST" (Share Tech Mono 6pt)
- **활성화 조건**: 자원 OnTriggerEnter → `gameObject.SetActive(true)`
- **비활성 시**: 알파 0 + 비활성

### 1️⃣2️⃣ 공격 버튼 (컨텍스트 — 우하단, 가장 큼)
- **Size**: 78 × 78, 원형 (`btn_attack.png`)
- **아이콘**: `icon_sword.png` (흰색)
- **라벨**: "ATTACK" (Oswald Bold 9pt)
- **활성화 조건**: 적 OnTriggerEnter → 활성화
- **이펙트**: 활성화 시 살짝 펄스 (Scale 1.0 ↔ 1.05, 1초 루프)

---

## ⚙️ Unity Import 설정

모든 PNG에 적용:
```
Texture Type:        Sprite (2D and UI)
Sprite Mode:         Single
Pixels Per Unit:     100
Mesh Type:           Tight
Filter Mode:         Bilinear
Compression:         None (또는 High Quality)
Generate Mip Maps:   ❌ off (UI는 끄기)
```

### 9-Slice 추천 (Sprite Editor → Border 설정)
- `panel_default.png` / `panel_highlight.png` → border **L:8 R:8 T:8 B:8** (16x16 총 32px)
- `panel_diagonal.png` → border **L:32 R:32 T:0 B:0**
- `quickslot_*` → border **L:6 R:6 T:6 B:6**

### Filled Image 설정 (게이지 바)
```
Image Type:          Filled
Fill Method:         Horizontal
Fill Origin:         Left
Fill Amount:         0~1 (스크립트로 제어)
```

---

## 🔌 권장 컴포넌트 매핑 (C# 의사코드)

```csharp
// HUDController.cs
public Image hpFill;              // hpbar_fill.png
public Image weightFill;          // weightbar_fill.png
public Image cycleFill;           // cycle_bar.png
public TMP_Text regionText;
public TMP_Text timeText;
public TMP_Text goldText;
public TMP_Text gemText;
public Button[] menuButtons;      // mail/shop/settings
public GameObject mailRedDot;     // red_dot.png
public Button[] quickSlots;       // 3개
public Button[] actionBar;        // INV/CRAFT/BUILD/MAP
public GameObject harvestButton;  // SetActive로 컨텍스트 토글
public GameObject attackButton;
public Joystick virtualJoystick;
```

---

## 📦 압축 다운로드

이 폴더 전체를 ZIP으로 다운로드해서 `Assets/UI/HUD_01/` 위치에 압축 해제하면 됩니다.

문의나 추가 에셋 필요하시면 알려주세요!
