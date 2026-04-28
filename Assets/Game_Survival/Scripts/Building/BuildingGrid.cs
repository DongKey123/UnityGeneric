using System.Collections.Generic;
using UnityEngine;

namespace SurvivalGame.Building
{
    /// <summary>
    /// 월드 공간의 1x1 그리드를 관리합니다.
    /// 건물 점유 여부 확인 및 월드 좌표 ↔ 셀 좌표 변환을 담당합니다.
    /// </summary>
    public class BuildingGrid : MonoBehaviour
    {
        #region Singleton

        public static BuildingGrid Instance { get; private set; }

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

        #region Private Fields

        private readonly Dictionary<Vector2Int, PlacedBuilding> _occupied = new();

        #endregion

        #region Public Methods

        /// <summary>월드 좌표를 그리드 셀 좌표로 변환합니다.</summary>
        public Vector2Int WorldToCell(Vector3 worldPos)
        {
            return new Vector2Int(Mathf.RoundToInt(worldPos.x), Mathf.RoundToInt(worldPos.z));
        }

        /// <summary>그리드 셀 좌표를 월드 좌표로 변환합니다. Y는 0으로 고정됩니다.</summary>
        public Vector3 CellToWorld(Vector2Int cell)
        {
            return new Vector3(cell.x, 0f, cell.y);
        }

        /// <summary>해당 셀 범위에 건물을 배치할 수 있는지 확인합니다.</summary>
        public bool CanPlace(Vector2Int origin, int width, int height)
        {
            for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
            {
                if (_occupied.ContainsKey(origin + new Vector2Int(x, z)))
                    return false;
            }
            return true;
        }

        /// <summary>건물을 그리드에 등록합니다.</summary>
        public void Register(Vector2Int origin, int width, int height, PlacedBuilding building)
        {
            for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
                _occupied[origin + new Vector2Int(x, z)] = building;
        }

        /// <summary>건물을 그리드에서 제거합니다.</summary>
        public void Unregister(Vector2Int origin, int width, int height)
        {
            for (int x = 0; x < width; x++)
            for (int z = 0; z < height; z++)
                _occupied.Remove(origin + new Vector2Int(x, z));
        }

        #endregion
    }
}
