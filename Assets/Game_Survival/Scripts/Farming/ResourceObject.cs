using Framework.Core.EventBus;
using SurvivalGame.Data;
using SurvivalGame.Inventories;
using SurvivalGame.Player;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SurvivalGame.Farming
{
    /// <summary>
    /// 씬에 배치되는 자원 오브젝트입니다.
    /// 플레이어가 범위에 진입하면 채집 버튼을 활성화하고,
    /// 내구도가 0이 되면 사라진 뒤 쿨타임 후 재생성됩니다.
    /// </summary>
    /// <remarks>
    /// [프리팹 구조]
    /// ResourceObject (루트 — ResourceObject 스크립트, Sphere Collider IsTrigger:true)
    /// └── Visual (자식 GameObject — 큐브/메시 등 비주얼, _visual 에 연결)
    ///       └── Box/Mesh Collider IsTrigger:false (물리 충돌용)
    ///
    /// Collider 두 개 구성:
    ///   - 루트의 Sphere Collider (IsTrigger:true)  → 채집 범위 감지
    ///   - Visual의 Collider      (IsTrigger:false) → 물리 충돌
    ///
    /// 채집 완료 시 Visual만 SetActive(false)하여 루트 오브젝트(스크립트/Invoke)는 유지합니다.
    /// </remarks>
    [RequireComponent(typeof(Collider))]
    public class ResourceObject : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private GameObject _visual;

        #endregion

        #region Private Fields

        private ResourceData     _data;
        private SurvivalItemData _itemData;
        private Inventory        _inventory;
        private Collider         _collider;
        private int              _currentDurability;
        private bool             _isRespawning;

        #endregion

        #region Properties

        public bool IsHarvestable => !_isRespawning && _currentDurability > 0;

        #endregion

        #region Unity Lifecycle

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        #endregion

        #region Public Methods

        /// <summary>ResourceSpawner에서 초기화 시 호출합니다.</summary>
        public void Initialize(ResourceData data, SurvivalItemData itemData, Inventory inventory)
        {
            _data       = data;
            _itemData   = itemData;
            _inventory  = inventory;
            _currentDurability = data.durability_max;
        }

        /// <summary>채집 버튼 클릭 시 호출합니다.</summary>
        public void Harvest()
        {
            if (!IsHarvestable) return;

            _currentDurability--;

            int dropCount = Random.Range(_data.drop_count_min, _data.drop_count_max + 1);
            _inventory.TryAdd(_itemData, dropCount);

            EventBus.Publish(new ResourceHarvestedEvent
            {
                ItemName = _data.name,
                Count    = dropCount
            });

            if (_currentDurability <= 0)
                StartRespawn();
        }

        #endregion

        #region Unity Lifecycle

        private void OnTriggerEnter(Collider other)
        {
            if (!IsHarvestable) return;
            if (!other.TryGetComponent<PlayerController>(out _)) return;

            EventBus.Publish(new HarvestRangeEnteredEvent { Resource = this });
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.TryGetComponent<PlayerController>(out _)) return;

            EventBus.Publish(new HarvestRangeExitedEvent { Resource = this });
        }

        #endregion

        #region Private Methods

        private void StartRespawn()
        {
            _isRespawning    = true;
            _collider.enabled = false;

            EventBus.Publish(new HarvestRangeExitedEvent { Resource = this });

            if (_visual != null) _visual.SetActive(false);

            Invoke(nameof(Respawn), _data.respawn_time);
        }

        private void Respawn()
        {
            _currentDurability = _data.durability_max;
            _isRespawning      = false;
            _collider.enabled  = true;

            if (_visual != null) _visual.SetActive(true);
        }

        #endregion
    }
}
