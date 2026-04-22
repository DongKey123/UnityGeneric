# 스킬 시스템 설계

---

## 타겟 방식

| 타입 | 설명 |
|------|------|
| `Single` | 단일 타겟 공격 |
| `AoE` | 원형 범위 공격 — `aoe_center` 필드로 중심 지정 |
| `Self` | 자기 자신 대상 (힐, 버프) |
| `Line` | 직선 관통 공격 |

> `All` 타입 없음 — AoE 반경을 크게 잡는 것으로 대체

### AoE 중심 (`aoe_center`)

| 값 | 설명 |
|----|------|
| `Self` | 플레이어 위치 중심 (내 주변 범위) |
| `Target` | 타겟 위치 중심 (타겟 주변 범위) |

> `aoe_center`는 `target_type == AoE` 일 때만 사용, 나머지 타입은 무시

---

## 효과 방식

| 타입 | 설명 |
|------|------|
| `Damage` | 즉발 피해 |
| `DoT` | 지속 피해 (독/화상 등) |
| `Buff` | 자신 스탯 강화 |
| `Debuff` | 적 스탯 약화 |
| `Shield` | 피해 흡수막 생성 |
| `Heal` | 체력 즉발 회복 |

> 버프/디버프는 별도 시스템 필요 → [BUFF_SYSTEM.md](BUFF_SYSTEM.md) ← 추후 작성

---

## 발동 방식

| 타입 | 설명 |
|------|------|
| `Instant` | 즉발 — 캐스팅 없이 즉시 발동 |
| `Cast` | 시전 시간 후 발동 (시전 중 이동 불가) |
| `Passive` | 조건 충족 시 자동 발동 |
| `Reaction` | 특정 조건 반응 (체력 X% 이하 등) |

---

## 스킬 데이터 (JSON 테이블)

| 필드 | 타입 | 설명 |
|------|------|------|
| `skill_id` | int | 스킬 고유 ID |
| `name` | string | 스킬 이름 |
| `description` | string | 스킬 설명 |
| `target_type` | string | Single / AoE / Self / Line |
| `aoe_center` | string | Self / Target (target_type == AoE 일 때만 사용) |
| `effect_type` | string | Damage / DoT / Buff / Debuff / Shield / Heal |
| `cast_type` | string | Instant / Cast / Passive / Reaction |
| `cooldown` | float | 쿨타임 (초) |
| `cast_time` | float | 시전 시간 (Instant는 0) |
| `damage_ratio` | float | 피해 배율 (ATK × ratio) |
| `range` | float | AoE 반경 / Line 길이 (Single·Self는 0) |
| `unlock_type` | string | Default(기본지급) / Condition(조건해금) |
| `unlock_condition` | string | 해금 조건 (추후 확정) |
| `max_enhance_level` | int | 최대 강화 단계 (추후 확정) |
| `enhance_target` | string | 강화 시 변경되는 필드명 (damage_ratio / range 등) |
| `enhance_value_per_level` | float | 강화 단계당 증가량 |

```json
// SkillData.json 예시
[
  {
    "skill_id": 1001,
    "name": "강타",
    "description": "단일 대상에게 강력한 일격을 가합니다.",
    "target_type": "Single",
    "aoe_center": "",
    "effect_type": "Damage",
    "cast_type": "Instant",
    "cooldown": 5.0,
    "cast_time": 0.0,
    "damage_ratio": 2.0,
    "range": 0.0,
    "unlock_type": "Default",
    "unlock_condition": "",
    "max_enhance_level": 10,
    "enhance_target": "damage_ratio",
    "enhance_value_per_level": 0.2
  },
  {
    "skill_id": 1002,
    "name": "폭발",
    "description": "플레이어 주변 범위에 폭발을 일으킵니다.",
    "target_type": "AoE",
    "aoe_center": "Self",
    "effect_type": "Damage",
    "cast_type": "Instant",
    "cooldown": 10.0,
    "cast_time": 0.0,
    "damage_ratio": 1.5,
    "range": 3.0,
    "unlock_type": "Condition",
    "unlock_condition": "",
    "max_enhance_level": 10,
    "enhance_target": "range",
    "enhance_value_per_level": 0.2
  }
]
```

---

## 스킬 강화

| 항목 | 내용 |
|------|------|
| 강화 재화 | 골드 (추후 전용 재화로 변경 가능) |
| 최대 강화 단계 | 미정 (추후 확정, 변경 가능) |

**강화 효과** — 스킬 종류에 따라 적용 가능한 강화 효과가 다름

| 강화 효과 | 설명 | 적용 가능 스킬 |
|-----------|------|----------------|
| 피해 배율 증가 | 기본 배율 × 강화 보너스 | Damage / DoT |
| 범위 증가 | AoE 반경 / Line 길이 확장 | AoE / Line |
| 쿨타임 감소 | 강화 단계당 쿨타임 감소 | 전체 |
| 지속시간 증가 | DoT / 버프 / 디버프 지속 연장 | DoT / Buff / Debuff |
| 흡수량 증가 | 실드 흡수량 증가 | Shield |
| 회복량 증가 | 힐량 증가 | Heal |

> 강화 효과는 스킬 데이터에서 어떤 항목을 강화할지 지정 — 스킬마다 다르게 설정 가능
> 강화 단계 / 재화 수치는 추후 확정

---

## 스킬 획득 방식

| 방식 | 설명 |
|------|------|
| 기본 지급 | 게임 시작 시 자동 보유 |
| 조건 해금 | 특정 조건 달성 시 해금 |

- 두 방식 혼재 — 스킬 데이터에서 `unlock_type` 필드로 구분
- 해금 조건 종류: 미정 (레벨 / 챕터 / 재화 구매 / 퀘스트 등 추후 확정)
- 슬롯 수 확장 가능 여부: 미정

---

## 구현 순서

1. [ ] 발동 방식 결정
2. [ ] 스킬 강화 / 획득 방식 결정
3. [ ] `SkillData.json` 필드 확정 및 작성
4. [ ] `SkillSystem` 클래스 구현
5. [ ] 버프/디버프 시스템 설계 → [BUFF_SYSTEM.md](BUFF_SYSTEM.md)
6. [ ] 스킬 버튼 UI 연동

---

## 관련 문서

- [전투 시스템](BATTLE_SYSTEM.md)
- [스탯 설계](STAT_DESIGN.md)
- [버프/디버프 시스템](BUFF_SYSTEM.md) ← 추후 작성
