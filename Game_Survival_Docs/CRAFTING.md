# 크래프팅 시스템 설계

> DESIGN.md 5번 항목(크래프팅)의 상세 설계 문서입니다.

---

## 크래프팅 방식

건물 조건 없음 — 재료만 보유하면 UI에서 언제든 제작 가능.

| 항목 | 내용 |
|------|------|
| 접근 방식 | 크래프팅 UI 버튼 → 레시피 목록 → 재료 충족 시 제작 |
| 건물 요구 | **없음** |
| 레시피 해금 | 플레이어 레벨 조건만 적용 (0 = 제한 없음) |
| 제작 시간 | **미정** (즉시 제작 vs 시간 소요) |

---

## 레시피 데이터 구조

```json
{
  "recipe_id": 1,
  "result_item_id": 2001,
  "result_count": 1,
  "required_level": 0,
  "ingredients": [
    { "item_id": 1001, "count": 10 }
  ]
}
```

| 필드 | 설명 |
|------|------|
| `required_level` | 해금에 필요한 플레이어 레벨 (0 = 제한 없음) |
| `ingredients` | 소모 재료 목록 |

---

## 현재 구현 상태

| 항목 | 상태 |
|------|------|
| `CraftingSystem` (재료 확인 + 소모 후 지급) | ✅ 완료 |
| `CraftingPanel` UI | ✅ 완료 |
| `Recipe.json` (Wooden Axe / Wooden Sword / Bandage 3종) | ✅ 완료 |
| `required_level` 필드 적용 및 체크 로직 | 🔲 예정 |

---

## 미결 사항

- [ ] `Recipe.json`에 `required_level` 필드 추가
- [ ] `CraftingSystem` — 플레이어 레벨 조건 체크 로직 추가
- [ ] 제작 시간 도입 여부
- [ ] 레시피 목록 확장 (건설 재료, 음식 등)

---

## 관련 문서

- [기획서](DESIGN.md)
- [건설 설계](BUILDING.md)
- [인벤토리 설계](INVENTORY.md)
- [작업 목록](TODO.md)
