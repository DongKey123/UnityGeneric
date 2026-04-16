# 작업 목록

---

## 설계 결정 (코드 작성 전 확정 필요)

- [x] 이동 방식 결정 — **NavMesh** 사용 (쿼터뷰 + 단순 필드 구조)
- [x] 챕터 전환 방식 결정 — **스테이지 전환**: 같은 씬 내 몬스터 교체 (로딩 없음) / **챕터 전환**: 씬 재로드 + 로딩 화면 (배경/맵 변경)
- [x] 몬스터 탐지 범위 결정 — `MonsterData.detect_range` 테이블로 몬스터별 관리
- [x] 스폰 포인트 방식 결정 — 스테이지 중심 기준 랜덤 반경 (`spawn_radius`), 플레이어 최소 거리 보장 (`spawn_min_distance_from_player`), NavMesh 위 유효 위치 검증
- [x] 최대 동시 몬스터 수 / 리스폰 시간 수치 확정 — `ChapterData.max_monster_count` / `respawn_delay` 테이블로 챕터별 관리
- [x] CombatState 분기 방식 — 단일 전투 상태 내에서 자동/수동 모드 if 분기
- [x] UI 연동 방식 — HP바: 머리 위 (월드스페이스), 데미지 숫자: 있음, 스킬 쿨타임: 버튼 오버레이 / 추후 설정에서 HUD ↔ 월드스페이스 전환 지원 예정
- [x] MP 시스템 여부 결정 — 없음 (스킬은 쿨타임으로만 관리)
- [x] 스탯 목록 확정 → [STAT_DESIGN.md](STAT_DESIGN.md)
- [ ] 스탯 계산 공식 설계 — 능력치 강화 / 문신 / 각인 스택 공식 (구현 시 작성)

---

## 핵심 시스템 구현

- [x] `IDamageable` 인터페이스
- [x] `PlayerStat` — 최종 스탯 계산 클래스 (순수 C#)
- [x] `PlayerController` + States (대기 / 전투 / 사망)
- [x] `MonsterController` + States (대기 / 추격 / 공격 / 사망)
- [x] `StageManager` — 몬스터 스폰 / 리스폰 / 챕터 관리
- [x] `MainEntry` — StageManager / PlayerController 초기화 연결

---

## 스킬 시스템

- [x] 타겟 방식 확정 — Single / AoE / Self / Line
- [x] 효과 방식 확정 — Damage / DoT / Buff / Debuff / Shield / Heal
- [x] 발동 방식 결정 — Instant / Cast / Passive / Reaction
- [x] 스킬 강화 방식 결정 — 골드 (추후 변경 가능), 강화 효과는 스킬마다 다르게 설정
- [x] 스킬 획득 방식 결정 — 기본 지급 + 조건 해금 혼재, 해금 조건 상세는 추후 확정
- [x] `SkillData.json` 필드 확정 및 작성
- [x] `SkillSystem` 클래스 구현
- [ ] 버프/디버프 시스템 설계 (BUFF_SYSTEM.md)
- [ ] 스킬 버튼 UI 연동

---

## UI 구현

- [ ] HUD — HP바, 골드/경험치, 스테이지 정보
- [ ] 스킬 버튼 (쿨타임 표시)
- [ ] 자동/수동 토글 버튼
- [ ] 캐릭터 정보 패널
- [ ] 강화 패널
- [ ] 설정 패널
- [ ] 오프라인 보상 팝업

---

## 시스템 연동

- [ ] 오프라인 보상 계산 (SaveSystem — 앱 종료 시각 기준)
- [ ] 저장 / 로드 (SaveSystem 연동)
- [ ] 오디오 (BGM / SFX)
- [ ] 다국어 (LocalizationSystem)

---

## 관련 문서

- [게임 기획서](IDLE_RPG_DESIGN.md)
- [전투 시스템 설계](BATTLE_SYSTEM.md)
- [스탯 설계](STAT_DESIGN.md)
