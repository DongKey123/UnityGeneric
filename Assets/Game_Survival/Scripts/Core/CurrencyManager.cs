using System.Collections.Generic;
using Framework.Core.EventBus;
using Framework.Core.SaveSystem;
using Framework.Core.Singleton;
using SurvivalGame.Defines;
using UnityEngine;

namespace SurvivalGame.Core
{
    /// <summary>
    /// 게임 내 재화(골드, 프리미엄)를 관리하는 매니저입니다.
    /// 씬 전환 후에도 유지되며, 변경 시 SaveSystem으로 자동 저장합니다.
    /// </summary>
    public class CurrencyManager : PersistentMonoSingleton<CurrencyManager>
    {
        #region Constants

        private const string SaveKey = "currency";

        #endregion

        #region Private Fields

        private Dictionary<CurrencyType, int> _amounts;

        #endregion

        #region Lifecycle

        protected override void OnInitialize()
        {
            Load();
        }

        #endregion

        #region Public Methods

        /// <summary>재화 현재 수량을 반환합니다.</summary>
        public int Get(CurrencyType type)
        {
            return _amounts.TryGetValue(type, out int value) ? value : 0;
        }

        /// <summary>재화가 <paramref name="amount"/> 이상 있는지 확인합니다.</summary>
        public bool Has(CurrencyType type, int amount)
        {
            return Get(type) >= amount;
        }

        /// <summary>
        /// 재화를 추가합니다.
        /// </summary>
        /// <param name="type">재화 종류</param>
        /// <param name="amount">추가할 양 (양수)</param>
        public void Add(CurrencyType type, int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[CurrencyManager] Add: amount must be positive. type={type}, amount={amount}");
                return;
            }

            _amounts[type] = Get(type) + amount;
            OnChanged(type, amount);
        }

        /// <summary>
        /// 재화를 소비합니다. 잔액이 부족하면 false를 반환하고 소비하지 않습니다.
        /// </summary>
        /// <param name="type">재화 종류</param>
        /// <param name="amount">소비할 양 (양수)</param>
        /// <returns>소비 성공 여부</returns>
        public bool Spend(CurrencyType type, int amount)
        {
            if (amount <= 0)
            {
                Debug.LogWarning($"[CurrencyManager] Spend: amount must be positive. type={type}, amount={amount}");
                return false;
            }

            if (!Has(type, amount))
            {
                Debug.Log($"[CurrencyManager] Spend 실패 — 잔액 부족. type={type}, 보유={Get(type)}, 필요={amount}");
                return false;
            }

            _amounts[type] = Get(type) - amount;
            OnChanged(type, -amount);
            return true;
        }

        #endregion

        #region Private Methods

        private void OnChanged(CurrencyType type, int delta)
        {
            Save();
            EventBus.Publish(new CurrencyChangedEvent
            {
                Type   = type,
                Amount = Get(type),
                Delta  = delta,
            });
        }

        private void Save()
        {
            SaveSystem.Save(SaveKey, _amounts);
        }

        private void Load()
        {
            _amounts = SaveSystem.Load<Dictionary<CurrencyType, int>>(SaveKey)
                       ?? new Dictionary<CurrencyType, int>
                       {
                           { CurrencyType.Gold,    0 },
                           { CurrencyType.Premium, 0 },
                       };
        }

        #endregion
    }
}
