using System.Collections.Generic;
using IdleGame.Data;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 플레이어 스킬 슬롯을 관리하고 스킬 발동을 처리합니다.
    /// PlayerController가 소유하며, Update(deltaTime)으로 쿨타임을 갱신합니다.
    /// </summary>
    public class SkillSystem
    {
        #region Constants

        private const string LayerMonster  = "Monster";
        private const float  LineHalfWidth = 0.5f;

        #endregion

        #region Private Fields

        private readonly List<SkillSlot> _slots = new();
        private readonly LayerMask       _monsterLayer;

        #endregion

        #region Constructor

        public SkillSystem()
        {
            _monsterLayer = LayerMask.GetMask(LayerMonster);
        }

        #endregion

        #region Properties

        /// <summary>현재 등록된 스킬 슬롯 목록 (읽기 전용)</summary>
        public IReadOnlyList<SkillSlot> Slots => _slots;

        #endregion

        #region Public Methods

        /// <summary>
        /// 스킬 슬롯을 초기화합니다.
        /// PlayerController.Initialize()에서 호출하세요.
        /// </summary>
        /// <param name="skills">장착할 스킬 데이터 목록</param>
        public void Initialize(IEnumerable<SkillData> skills)
        {
            _slots.Clear();
            foreach (var data in skills)
                _slots.Add(new SkillSlot(data));
        }

        /// <summary>쿨타임을 갱신합니다. PlayerController.Update()에서 호출하세요.</summary>
        /// <param name="deltaTime">Time.deltaTime</param>
        public void Tick(float deltaTime)
        {
            foreach (var slot in _slots)
                slot.TickCooldown(deltaTime);
        }

        /// <summary>
        /// 쿨타임이 완료된 첫 번째 스킬을 자동으로 선택해 발동합니다.
        /// 발동 성공 시 true를 반환합니다.
        /// </summary>
        /// <param name="owner">스킬 사용자</param>
        public bool TryUseAny(PlayerController owner)
        {
            foreach (var slot in _slots)
            {
                if (!slot.IsReady) continue;
                if (slot.Data.cast_type == "Passive" || slot.Data.cast_type == "Reaction") continue;

                return TryUseSlot(slot, owner);
            }
            return false;
        }

        /// <summary>
        /// 특정 인덱스의 스킬을 발동합니다.
        /// 발동 성공 시 true를 반환합니다.
        /// </summary>
        /// <param name="index">슬롯 인덱스</param>
        /// <param name="owner">스킬 사용자</param>
        public bool TryUse(int index, PlayerController owner)
        {
            if (index < 0 || index >= _slots.Count) return false;
            return TryUseSlot(_slots[index], owner);
        }

        #endregion

        #region Private Methods

        private bool TryUseSlot(SkillSlot slot, PlayerController owner)
        {
            if (!slot.IsReady) return false;

            bool executed = slot.Data.target_type switch
            {
                "Single" => ExecuteSingle(slot, owner),
                "AoE"    => ExecuteAoE(slot, owner),
                "Self"   => ExecuteSelf(slot, owner),
                "Line"   => ExecuteLine(slot, owner),
                _        => false
            };

            if (executed)
                slot.StartCooldown();

            return executed;
        }

        /// <summary>단일 타겟에게 스킬을 적용합니다.</summary>
        private bool ExecuteSingle(SkillSlot slot, PlayerController owner)
        {
            if (owner.Target == null || owner.Target.IsDead) return false;

            ApplyEffect(slot, owner, owner.Target);
            return true;
        }

        /// <summary>범위 내 모든 타겟에게 스킬을 적용합니다.</summary>
        private bool ExecuteAoE(SkillSlot slot, PlayerController owner)
        {
            Vector3 center = slot.Data.aoe_center == "Target" && owner.TargetTransform != null
                ? owner.TargetTransform.position
                : owner.transform.position;

            float range = slot.GetRange();
            Collider[] hits = Physics.OverlapSphere(center, range, _monsterLayer);

            bool hit = false;
            foreach (var col in hits)
            {
                var damageable = col.GetComponent<IDamageable>();
                if (damageable == null || damageable.IsDead) continue;

                ApplyEffect(slot, owner, damageable);
                hit = true;
            }
            return hit;
        }

        /// <summary>자기 자신에게 스킬을 적용합니다.</summary>
        private bool ExecuteSelf(SkillSlot slot, PlayerController owner)
        {
            ApplySelfEffect(slot, owner);
            return true;
        }

        /// <summary>직선 방향으로 적을 관통합니다.</summary>
        private bool ExecuteLine(SkillSlot slot, PlayerController owner)
        {
            if (owner.TargetTransform == null) return false;

            Vector3 origin    = owner.transform.position;
            Vector3 direction = (owner.TargetTransform.position - origin).normalized;
            float   range     = slot.GetRange();

            RaycastHit[] hits = Physics.SphereCastAll(origin, LineHalfWidth, direction, range, _monsterLayer);

            bool hit = false;
            foreach (var rayHit in hits)
            {
                var damageable = rayHit.collider.GetComponent<IDamageable>();
                if (damageable == null || damageable.IsDead) continue;

                ApplyEffect(slot, owner, damageable);
                hit = true;
            }
            return hit;
        }

        /// <summary>타겟에게 effect_type에 따라 효과를 적용합니다.</summary>
        private void ApplyEffect(SkillSlot slot, PlayerController owner, IDamageable target)
        {
            switch (slot.Data.effect_type)
            {
                case "Damage":
                case "DoT":
                    int power = Mathf.RoundToInt(owner.Stat.Atk * slot.GetDamageRatio());
                    target.TakeDamage(power);
                    break;

                // TODO: Buff / Debuff → BUFF_SYSTEM 연동
                // TODO: Shield → PlayerController 실드 필드 연동
            }
        }

        /// <summary>자신에게 effect_type에 따라 효과를 적용합니다.</summary>
        private void ApplySelfEffect(SkillSlot slot, PlayerController owner)
        {
            switch (slot.Data.effect_type)
            {
                case "Heal":
                    int healAmount = Mathf.RoundToInt(owner.Stat.Atk * slot.GetDamageRatio());
                    owner.Heal(healAmount);
                    break;

                // TODO: Buff → BUFF_SYSTEM 연동
                // TODO: Shield → PlayerController 실드 필드 연동
            }
        }

        #endregion
    }
}
