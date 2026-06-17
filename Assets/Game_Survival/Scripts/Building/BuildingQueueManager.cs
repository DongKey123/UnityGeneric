using System.Collections.Generic;
using SurvivalGame.Data;
using UnityEngine;

namespace SurvivalGame.Building
{
    /// <summary>
    /// 건물 제작 큐를 관리하는 싱글턴입니다.
    /// 건물 "건설" 클릭 시 AddToQueue로 등록하고, 배치 확정 시 SetPlaced로 상태를 전이합니다.
    /// </summary>
    public class BuildingQueueManager : MonoBehaviour
    {
        public static BuildingQueueManager Instance { get; private set; }

        private readonly List<BuildingQueueEntry> _queue = new();

        public event System.Action OnQueueChanged;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void AddToQueue(BuildingData data)
        {
            _queue.Add(new BuildingQueueEntry(data));
            OnQueueChanged?.Invoke();
        }

        public void SetPlaced(BuildingQueueEntry entry, PlacedBuilding placed)
        {
            entry.MarkPlaced(placed);
            OnQueueChanged?.Invoke();
        }

        public void Remove(BuildingQueueEntry entry)
        {
            _queue.Remove(entry);
            OnQueueChanged?.Invoke();
        }

        public IReadOnlyList<BuildingQueueEntry> GetAll() => _queue;

        public void NotifyChanged() => OnQueueChanged?.Invoke();
    }
}
