using System.Collections.Generic;
using System.Linq;
using Framework.UI;
using SurvivalGame.Battle;
using SurvivalGame.Core;
using SurvivalGame.Defines;
using SurvivalGame.Farming;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalGame.UI
{
    /// <summary>
    /// 인게임 미니맵 서브 패널입니다.
    /// MapManager에서 엔티티 위치를 읽어 dot Image로 표시합니다.
    /// MainPanel의 자식으로 배치하고 OnOpened/OnClosed에서 Show/Hide를 호출하세요.
    /// </summary>
    public class MinimapSubPanel : SubPanel
    {
        #region Inspector

        [SerializeField] private RectTransform _playerDot;
        [SerializeField] private Image         _enemyDotPrefab;
        [SerializeField] private Image         _resourceDotPrefab;
        [SerializeField] private RectTransform _markerContainer;
        [SerializeField] private float         _minimapRadius = 78f;

        #endregion

        #region Fields

        private readonly Dictionary<Enemy, RectTransform>          _enemyDots    = new();
        private readonly Dictionary<ResourceObject, RectTransform> _resourceDots = new();

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            SyncDots();
            UpdatePositions();
        }

        #endregion

        #region SubPanel Lifecycle

        protected override void OnHidden()
        {
            foreach (var dot in _enemyDots.Values)    Destroy(dot.gameObject);
            foreach (var dot in _resourceDots.Values) Destroy(dot.gameObject);
            _enemyDots.Clear();
            _resourceDots.Clear();
        }

        #endregion

        #region Private Methods

        private void SyncDots()
        {
            SyncEnemyDots();
            SyncResourceDots();
        }

        private void SyncEnemyDots()
        {
            var activeEnemies = MapManager.Instance.Enemies;

            // 사망·해제된 적 dot 제거
            var stale = new List<Enemy>();
            foreach (var key in _enemyDots.Keys)
                if (!activeEnemies.Contains(key))
                    stale.Add(key);

            foreach (var e in stale)
            {
                Destroy(_enemyDots[e].gameObject);
                _enemyDots.Remove(e);
            }

            // 새로 등록된 적 dot 추가
            if (_enemyDotPrefab == null) return;
            foreach (var enemy in activeEnemies)
            {
                if (_enemyDots.ContainsKey(enemy)) continue;
                var dot = Instantiate(_enemyDotPrefab, _markerContainer);
                _enemyDots[enemy] = dot.rectTransform;
            }
        }

        private void SyncResourceDots()
        {
            if (_resourceDotPrefab == null) return;

            foreach (var resource in MapManager.Instance.Resources)
            {
                if (!_resourceDots.ContainsKey(resource))
                {
                    var dot = Instantiate(_resourceDotPrefab, _markerContainer);
                    _resourceDots[resource] = dot.rectTransform;
                }

                // 리스폰 중에는 dot 숨김
                _resourceDots[resource].gameObject.SetActive(resource.IsHarvestable);
            }
        }

        private void UpdatePositions()
        {
            var playerTransform = MapManager.Instance.PlayerTransform;
            if (playerTransform == null) return;

            var playerPos = playerTransform.position;

            foreach (var kvp in _enemyDots)
                SetDotPosition(kvp.Value, kvp.Key.transform.position, playerPos);

            foreach (var kvp in _resourceDots)
            {
                if (!kvp.Value.gameObject.activeSelf) continue;
                SetDotPosition(kvp.Value, kvp.Key.transform.position, playerPos);
            }
        }

        private void SetDotPosition(RectTransform dot, Vector3 worldPos, Vector3 playerPos)
        {
            var rel    = worldPos - playerPos;
            var uiPos  = new Vector2(rel.x, rel.z) / GameConsts.MinimapDisplayRadius * _minimapRadius;
            var sqrMax = _minimapRadius * _minimapRadius;

            if (uiPos.sqrMagnitude > sqrMax)
                uiPos = uiPos.normalized * _minimapRadius;

            dot.anchoredPosition = uiPos;
        }

        #endregion
    }
}
