using System;
using System.Collections.Generic;
using Framework.Core.Singleton;
using Newtonsoft.Json;
using UnityEngine;

namespace Framework.Core.DataManager
{
    /// <summary>
    /// JSON 파일을 로드하고 캐싱하는 런타임 게임 데이터 매니저입니다.
    /// Resources 폴더 기반으로 동작하며, ExcelToJson으로 변환된 JSON 파일 로드에 최적화되어 있습니다.
    ///
    /// [사용 흐름]
    /// 1. LoadAsDictionary 로 데이터 로드 (id 기반 빠른 조회)
    /// 2. Get 으로 특정 데이터 접근
    /// 3. 씬 전환 또는 불필요 시 Unload 로 캐시 해제
    ///
    /// [Resources 경로 규칙]
    /// Resources 폴더 기준 경로, 확장자 제외
    /// 예) "Data/Monster" → Assets/Resources/Data/Monster.json
    /// </summary>
    public class InGameDataManager : Singleton<InGameDataManager>
    {
        #region Fields

        private readonly Dictionary<string, object> _listCache = new();
        private readonly Dictionary<string, object> _dictCache = new();

        #endregion

        #region Initialization

        /// <summary>
        /// 플레이 모드 재시작 시 캐시를 초기화합니다.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            Release();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// JSON 파일을 로드하여 List 형태로 반환합니다. 이미 로드된 경우 캐시를 반환합니다.
        /// </summary>
        /// <typeparam name="T">데이터 타입</typeparam>
        /// <param name="resourcePath">Resources 기준 경로 (확장자 제외, 예: "Data/Monster")</param>
        /// <returns>데이터 리스트. 파일이 없으면 null</returns>
        public List<T> Load<T>(string resourcePath)
        {
            if (_listCache.TryGetValue(resourcePath, out var cached))
                return (List<T>)cached;

            var json = LoadJson(resourcePath);
            if (json == null)
                return null;

            var list = JsonConvert.DeserializeObject<List<T>>(json);
            _listCache[resourcePath] = list;

#if UNITY_EDITOR
            Debug.Log($"[InGameDataManager] Loaded: {resourcePath} ({list?.Count ?? 0} rows)");
#endif

            return list;
        }

        /// <summary>
        /// JSON 파일을 로드하여 Dictionary 형태로 반환합니다. 이미 로드된 경우 캐시를 반환합니다.
        /// </summary>
        /// <typeparam name="TKey">키 타입</typeparam>
        /// <typeparam name="TValue">값 타입</typeparam>
        /// <param name="resourcePath">Resources 기준 경로 (확장자 제외)</param>
        /// <param name="keySelector">키로 사용할 필드 지정 (예: x => x.id)</param>
        /// <returns>Dictionary. 파일이 없으면 null</returns>
        public Dictionary<TKey, TValue> LoadAsDictionary<TKey, TValue>(string resourcePath, Func<TValue, TKey> keySelector)
        {
            if (_dictCache.TryGetValue(resourcePath, out var cached))
                return (Dictionary<TKey, TValue>)cached;

            var json = LoadJson(resourcePath);
            if (json == null)
                return null;

            var list = JsonConvert.DeserializeObject<List<TValue>>(json);
            if (list == null)
                return null;

            var dict = new Dictionary<TKey, TValue>(list.Count);
            foreach (var item in list)
                dict[keySelector(item)] = item;

            _dictCache[resourcePath] = dict;

#if UNITY_EDITOR
            Debug.Log($"[InGameDataManager] Loaded as Dictionary: {resourcePath} ({dict.Count} entries)");
#endif

            return dict;
        }

        /// <summary>
        /// LoadAsDictionary로 캐시된 데이터에서 특정 키의 값을 반환합니다.
        /// </summary>
        /// <typeparam name="TKey">키 타입</typeparam>
        /// <typeparam name="TValue">값 타입</typeparam>
        /// <param name="resourcePath">LoadAsDictionary 시 사용한 경로</param>
        /// <param name="key">조회할 키</param>
        /// <returns>해당 키의 데이터. 없으면 default</returns>
        public TValue Get<TKey, TValue>(string resourcePath, TKey key)
        {
            if (!_dictCache.TryGetValue(resourcePath, out var cached))
            {
                Debug.LogWarning($"[InGameDataManager] Get 실패 — '{resourcePath}'가 LoadAsDictionary로 로드되지 않았습니다.");
                return default;
            }

            var dict = (Dictionary<TKey, TValue>)cached;

            if (dict.TryGetValue(key, out var value))
                return value;

            Debug.LogWarning($"[InGameDataManager] Get 실패 — '{resourcePath}'에서 키 '{key}'를 찾을 수 없습니다.");
            return default;
        }

        /// <summary>
        /// 특정 경로의 캐시를 해제합니다.
        /// </summary>
        /// <param name="resourcePath">해제할 경로</param>
        public void Unload(string resourcePath)
        {
            _listCache.Remove(resourcePath);
            _dictCache.Remove(resourcePath);

#if UNITY_EDITOR
            Debug.Log($"[InGameDataManager] Unloaded: {resourcePath}");
#endif
        }

        /// <summary>
        /// 모든 캐시를 해제합니다.
        /// </summary>
        public void UnloadAll()
        {
            _listCache.Clear();
            _dictCache.Clear();

#if UNITY_EDITOR
            Debug.Log("[InGameDataManager] All cache cleared.");
#endif
        }

        #endregion

        #region Private Methods

        private static string LoadJson(string resourcePath)
        {
            var asset = Resources.Load<TextAsset>(resourcePath);

            if (asset != null)
                return asset.text;

            Debug.LogError($"[InGameDataManager] 파일을 찾을 수 없습니다: Resources/{resourcePath}.json");
            return null;
        }

        #endregion
    }
}
