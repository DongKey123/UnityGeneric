using System.Collections.Generic;
using Framework.Core.Singleton;
using SurvivalGame.Battle;
using SurvivalGame.Farming;
using SurvivalGame.Player;
using UnityEngine;

namespace SurvivalGame.Core
{
    /// <summary>
    /// 현재 씬에 존재하는 엔티티(플레이어·적·자원)의 위치 정보를 관리합니다.
    /// 미니맵, 타겟팅 등 월드 정보가 필요한 시스템의 단일 데이터 소스입니다.
    /// 서버 연동 시 NetworkLayer가 이 매니저에 엔티티를 등록·해제합니다.
    /// </summary>
    public class MapManager : Singleton<MapManager>
    {
        #region Fields

        private readonly List<Enemy>          _enemies   = new();
        private readonly List<ResourceObject> _resources = new();

        #endregion

        #region Properties

        /// <summary>플레이어 Transform입니다.</summary>
        public Transform PlayerTransform { get; private set; }

        /// <summary>현재 씬에 등록된 적 목록입니다.</summary>
        public IReadOnlyList<Enemy> Enemies => _enemies;

        /// <summary>현재 씬에 등록된 자원 목록입니다.</summary>
        public IReadOnlyList<ResourceObject> Resources => _resources;

        #endregion

        #region Public Methods

        /// <summary>플레이어를 등록합니다. SurvivalEntry.Start()에서 호출하세요.</summary>
        public void SetPlayer(PlayerController player)
        {
            PlayerTransform = player.transform;
        }

        /// <summary>적을 목록에 추가합니다. EnemySpawner에서 스폰 시 호출하세요.</summary>
        public void RegisterEnemy(Enemy enemy)
        {
            if (!_enemies.Contains(enemy))
                _enemies.Add(enemy);
        }

        /// <summary>적을 목록에서 제거합니다. 사망 시 EnemySpawner에서 호출하세요.</summary>
        public void UnregisterEnemy(Enemy enemy)
        {
            _enemies.Remove(enemy);
        }

        /// <summary>자원을 목록에 추가합니다. ResourceSpawner에서 스폰 시 호출하세요.</summary>
        public void RegisterResource(ResourceObject resource)
        {
            if (!_resources.Contains(resource))
                _resources.Add(resource);
        }

        #endregion

        protected override void OnInitialize()
        {
            _enemies.Clear();
            _resources.Clear();
            PlayerTransform = null;
        }
    }
}
