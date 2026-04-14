using System;
using IdleGame.Data;
using UnityEngine;

namespace IdleGame.Battle
{
    /// <summary>
    /// 플레이어의 최종 스탯을 계산하고 보관합니다.
    /// 장비 / 능력치 강화 / 문신을 합산한 결과를 제공합니다.
    /// 스탯 변경 시 <see cref="OnStatChanged"/> 이벤트로 UI에 알립니다.
    /// </summary>
    public class PlayerStat
    {
        #region Events

        /// <summary>스탯이 재계산될 때 발생합니다.</summary>
        public event Action OnStatChanged;

        #endregion

        #region Final Stats (Read-only)

        public int MaxHp { get; private set; }
        public int Atk { get; private set; }
        public int Def { get; private set; }
        public float AtkSpeed { get; private set; }
        public float AtkRange { get; private set; }
        public float MoveSpeed { get; private set; }
        public float CritRate { get; private set; }
        public float CritDmg { get; private set; }
        public float ExpBonus { get; private set; }
        public float GoldBonus { get; private set; }
        public float ItemDropRate { get; private set; }
        public float IdleGoldBonus { get; private set; }
        public float IdleExpBonus { get; private set; }

        #endregion

        #region Equipment

        private ItemData _weapon;
        private ItemData _helmet;
        private ItemData _armor;
        private ItemData _gloves;
        private ItemData _boots;

        private int _weaponLevel;
        private int _helmetLevel;
        private int _armorLevel;
        private int _glovesLevel;
        private int _bootsLevel;

        #endregion

        #region Enhancement (능력치 강화)

        private int _enhanceMaxHp;
        private int _enhanceAtk;
        private int _enhanceDef;
        private float _enhanceAtkSpeed;
        private float _enhanceMoveSpeed;
        private float _enhanceCritRate;
        private float _enhanceCritDmg;
        private float _enhanceExpBonus;
        private float _enhanceGoldBonus;
        private float _enhanceItemDropRate;
        private float _enhanceIdleGoldBonus;
        private float _enhanceIdleExpBonus;

        #endregion

        #region Constants

        private const float BaseAtkSpeed  = 1.0f;
        private const float BaseMoveSpeed = 3.0f;
        private const float BaseCritRate  = 0.05f;  // 5%
        private const float BaseCritDmg   = 2.0f;   // 2.0배
        private const float BaseAtkRange  = 1.5f;   // 장비 없을 때 기본 사거리

        #endregion

        #region Public Methods

        /// <summary>
        /// 장비를 설정하고 스탯을 재계산합니다.
        /// </summary>
        /// <param name="item">장착할 장비 데이터</param>
        /// <param name="enhanceLevel">강화 단계 (+0 ~ +10)</param>
        public void SetEquipment(ItemData item, int enhanceLevel = 0)
        {
            if (item == null) return;

            switch (item.slot_type)
            {
                case "weapon":  _weapon = item;  _weaponLevel = enhanceLevel;  break;
                case "helmet":  _helmet = item;  _helmetLevel = enhanceLevel;  break;
                case "armor":   _armor = item;   _armorLevel = enhanceLevel;   break;
                case "gloves":  _gloves = item;  _glovesLevel = enhanceLevel;  break;
                case "boots":   _boots = item;   _bootsLevel = enhanceLevel;   break;
                default:
                    Debug.LogWarning($"[PlayerStat] 알 수 없는 슬롯 타입: {item.slot_type}");
                    return;
            }

            Recalculate();
        }

        /// <summary>
        /// 능력치 강화 수치를 설정하고 스탯을 재계산합니다.
        /// </summary>
        public void SetEnhancement(
            int maxHp = 0, int atk = 0, int def = 0,
            float atkSpeed = 0f, float moveSpeed = 0f,
            float critRate = 0f, float critDmg = 0f,
            float expBonus = 0f, float goldBonus = 0f,
            float itemDropRate = 0f, float idleGoldBonus = 0f, float idleExpBonus = 0f)
        {
            _enhanceMaxHp        = maxHp;
            _enhanceAtk          = atk;
            _enhanceDef          = def;
            _enhanceAtkSpeed     = atkSpeed;
            _enhanceMoveSpeed    = moveSpeed;
            _enhanceCritRate     = critRate;
            _enhanceCritDmg      = critDmg;
            _enhanceExpBonus     = expBonus;
            _enhanceGoldBonus    = goldBonus;
            _enhanceItemDropRate = itemDropRate;
            _enhanceIdleGoldBonus = idleGoldBonus;
            _enhanceIdleExpBonus  = idleExpBonus;

            Recalculate();
        }

        /// <summary>
        /// 현재 장비 + 능력치 강화를 합산하여 최종 스탯을 재계산합니다.
        /// 장비 교체 또는 강화 후 반드시 호출하세요.
        /// </summary>
        public void Recalculate()
        {
            MaxHp = CalcEquipStat(_helmet, _helmetLevel, i => i.max_hp)
                  + CalcEquipStat(_armor,  _armorLevel,  i => i.max_hp)
                  + _enhanceMaxHp;

            Atk = CalcEquipStat(_weapon, _weaponLevel, i => i.atk)
                + _enhanceAtk;

            Def = CalcEquipStat(_armor,  _armorLevel,  i => i.def)
                + CalcEquipStat(_gloves, _glovesLevel, i => i.def)
                + CalcEquipStat(_boots,  _bootsLevel,  i => i.def)
                + _enhanceDef;

            AtkSpeed = BaseAtkSpeed
                     + CalcEquipStatF(_gloves, _glovesLevel, i => i.atk_speed)
                     + _enhanceAtkSpeed;

            AtkRange = _weapon != null ? _weapon.atk_range : BaseAtkRange;

            MoveSpeed = BaseMoveSpeed
                      + CalcEquipStatF(_boots, _bootsLevel, i => i.move_speed)
                      + _enhanceMoveSpeed;

            CritRate = BaseCritRate + _enhanceCritRate;
            CritDmg  = BaseCritDmg  + _enhanceCritDmg;

            ExpBonus      = _enhanceExpBonus;
            GoldBonus     = _enhanceGoldBonus;
            ItemDropRate  = _enhanceItemDropRate;
            IdleGoldBonus = _enhanceIdleGoldBonus;
            IdleExpBonus  = _enhanceIdleExpBonus;

            OnStatChanged?.Invoke();
        }

        #endregion

        #region Private Methods

        /// <summary>강화 단계를 반영한 장비 정수 스탯을 반환합니다. 단계당 기본 스탯 10% 추가.</summary>
        private int CalcEquipStat(ItemData item, int level, Func<ItemData, int> selector)
        {
            if (item == null) return 0;
            int base_ = selector(item);
            return base_ + Mathf.RoundToInt(base_ * level * 0.1f);
        }

        /// <summary>강화 단계를 반영한 장비 실수 스탯을 반환합니다. 단계당 기본 스탯 10% 추가.</summary>
        private float CalcEquipStatF(ItemData item, int level, Func<ItemData, float> selector)
        {
            if (item == null) return 0f;
            float base_ = selector(item);
            return base_ + base_ * level * 0.1f;
        }

        #endregion
    }
}
