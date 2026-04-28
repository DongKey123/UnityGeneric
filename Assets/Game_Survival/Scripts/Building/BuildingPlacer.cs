using SurvivalGame.Data;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using UnityEngine;

namespace SurvivalGame.Building
{
    /// <summary>
    /// 건물 배치 모드를 관리합니다.
    /// 플레이어 정면에 고스트(미리보기)를 표시하고, Place / Cancel 입력을 처리합니다.
    /// </summary>
    public class BuildingPlacer : MonoBehaviour
    {
        #region Singleton

        public static BuildingPlacer Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        #endregion

        #region Inspector

        [SerializeField] private float _previewDistance = 2f;

        #endregion

        #region Private Fields

        private BuildingData _currentData;
        private Inventory    _inventory;
        private PlayerController _player;
        private GameObject   _ghost;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");

        #endregion

        #region Properties

        public bool IsPlacing { get; private set; }

        #endregion

        #region Unity Lifecycle

        private void Update()
        {
            if (!IsPlacing) return;
            UpdateGhostPosition();
        }

        #endregion

        #region Public Methods

        /// <summary>배치 모드를 시작합니다. BuildModePanel에서 호출합니다.</summary>
        public void EnterPlacementMode(BuildingData data, PlayerController player, Inventory inventory)
        {
            _currentData = data;
            _player      = player;
            _inventory   = inventory;
            IsPlacing    = true;

            CreateGhost(data);
        }

        /// <summary>배치를 확정합니다. Place 버튼에서 호출합니다.</summary>
        public bool TryPlace()
        {
            if (!IsPlacing) return false;

            Vector2Int cell = GetTargetCell();

            if (!BuildingGrid.Instance.CanPlace(cell, _currentData.grid_width, _currentData.grid_height))
                return false;

            if (!HasResources())
                return false;

            ConsumeResources();
            SpawnBuilding(cell);
            ExitPlacementMode();
            return true;
        }

        /// <summary>배치 모드를 취소합니다. Cancel 버튼에서 호출합니다.</summary>
        public void ExitPlacementMode()
        {
            IsPlacing    = false;
            _currentData = null;

            if (_ghost != null)
            {
                Destroy(_ghost);
                _ghost = null;
            }
        }

        /// <summary>현재 고스트 위치가 배치 가능한지 반환합니다.</summary>
        public bool CanPlaceAtCurrentPosition()
        {
            if (!IsPlacing) return false;
            Vector2Int cell = GetTargetCell();
            return BuildingGrid.Instance.CanPlace(cell, _currentData.grid_width, _currentData.grid_height)
                   && HasResources();
        }

        #endregion

        #region Private Methods

        private void UpdateGhostPosition()
        {
            if (_ghost == null) return;

            Vector3 worldPos = GetPreviewWorldPosition();
            _ghost.transform.position = worldPos;

            bool valid = CanPlaceAtCurrentPosition();
            SetGhostColor(valid ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f));
        }

        private Vector3 GetPreviewWorldPosition()
        {
            Vector3 forward = _player.transform.forward;
            forward.y = 0f;
            if (forward == Vector3.zero) forward = Vector3.forward;

            Vector3 raw = _player.transform.position + forward.normalized * _previewDistance;
            Vector2Int cell = BuildingGrid.Instance.WorldToCell(raw);
            return BuildingGrid.Instance.CellToWorld(cell);
        }

        private Vector2Int GetTargetCell()
        {
            Vector3 raw = _player.transform.position
                          + _player.transform.forward.normalized * _previewDistance;
            raw.y = 0f;
            return BuildingGrid.Instance.WorldToCell(raw);
        }

        private void CreateGhost(BuildingData data)
        {
            var prefab = Resources.Load<GameObject>(data.prefab_path);
            if (prefab == null)
            {
                Debug.LogWarning($"[BuildingPlacer] 프리팹을 찾을 수 없습니다: {data.prefab_path}");
                return;
            }

            _ghost = Instantiate(prefab);

            // 콜라이더 비활성화 — 고스트는 물리 충돌 없음
            foreach (var col in _ghost.GetComponentsInChildren<Collider>())
                col.enabled = false;

            SetGhostColor(new Color(0f, 1f, 0f, 0.5f));
        }

        private void SetGhostColor(Color color)
        {
            foreach (var renderer in _ghost.GetComponentsInChildren<Renderer>())
            {
                // 인스턴스 머티리얼을 직접 수정해 원본에 영향을 주지 않음
                renderer.material.SetColor(BaseColorId, color);
            }
        }

        private bool HasResources()
        {
            foreach (var cost in _currentData.costs)
            {
                if (_inventory.GetCount(cost.item_id) < cost.count)
                    return false;
            }
            return true;
        }

        private void ConsumeResources()
        {
            foreach (var cost in _currentData.costs)
            {
                var itemData = InGameDataManager.Instance.Get<SurvivalItemData>(cost.item_id);
                if (itemData != null)
                    _inventory.TryRemove(itemData, cost.count);
            }
        }

        private void SpawnBuilding(Vector2Int cell)
        {
            var prefab = Resources.Load<GameObject>(_currentData.prefab_path);
            if (prefab == null) return;

            Vector3 worldPos = BuildingGrid.Instance.CellToWorld(cell);
            var go           = Instantiate(prefab, worldPos, Quaternion.identity);
            var placed       = go.AddComponent<PlacedBuilding>();
            placed.Initialize(_currentData, cell);

            BuildingGrid.Instance.Register(cell, _currentData.grid_width, _currentData.grid_height, placed);
        }

        #endregion
    }
}
