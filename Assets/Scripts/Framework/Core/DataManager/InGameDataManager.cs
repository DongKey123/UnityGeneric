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
    /// 2. 반환된 Dictionary를 직접 사용
    /// 3. 씬 전환 또는 불필요 시 Unload 로 캐시 해제
    ///
    /// [Resources 경로 규칙]
    /// Resources 폴더 기준 경로, 확장자 제외
    /// 예) "Data/Monster" → Assets/Resources/Data/Monster.json
    /// </summary>
    public partial class InGameDataManager : Singleton<InGameDataManager>
    {
        #region Cache

        private static class ListCache<T>
        {
            internal static readonly Dictionary<string, List<T>> Store = new();
        }

        private static class DictCache<TKey, TValue>
        {
            internal static readonly Dictionary<string, Dictionary<TKey, TValue>> Store = new();
        }

        // UnloadAll 지원을 위해 등록된 캐시 클리어 액션 목록 (경로 → 액션)
        private static readonly Dictionary<string, Action> _clearActions = new();

        #endregion

        #region Initialization

        /// <summary>
        /// 플레이 모드 재시작 시 모든 캐시를 초기화합니다.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Init()
        {
            foreach (var action in _clearActions.Values)
                action();

            _clearActions.Clear();
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
            if (ListCache<T>.Store.TryGetValue(resourcePath, out var cached))
                return cached;

            var json = LoadJson(resourcePath);
            if (json == null)
                return null;

            var list = JsonConvert.DeserializeObject<List<T>>(json);
            if (list == null)
                return null;

            ListCache<T>.Store[resourcePath] = list;
            _clearActions[$"L:{typeof(T).FullName}:{resourcePath}"] = () => ListCache<T>.Store.Remove(resourcePath);

#if UNITY_EDITOR
            Debug.Log($"[InGameDataManager] Loaded: {resourcePath} ({list.Count} rows)");
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
            if (DictCache<TKey, TValue>.Store.TryGetValue(resourcePath, out var cached))
                return cached;

            var json = LoadJson(resourcePath);
            if (json == null)
                return null;

            var list = JsonConvert.DeserializeObject<List<TValue>>(json);
            if (list == null)
                return null;

            var dict = new Dictionary<TKey, TValue>(list.Count);
            foreach (var item in list)
                dict[keySelector(item)] = item;

            DictCache<TKey, TValue>.Store[resourcePath] = dict;
            _clearActions[$"D:{typeof(TKey).FullName}:{typeof(TValue).FullName}:{resourcePath}"] = () => DictCache<TKey, TValue>.Store.Remove(resourcePath);

#if UNITY_EDITOR
            Debug.Log($"[InGameDataManager] Loaded as Dictionary: {resourcePath} ({dict.Count} entries)");
#endif
            return dict;
        }

        /// <summary>
        /// List로 로드된 특정 경로의 캐시를 해제합니다.
        /// </summary>
        /// <typeparam name="T">로드 시 사용한 데이터 타입</typeparam>
        /// <param name="resourcePath">해제할 경로</param>
        public void Unload<T>(string resourcePath)
        {
            ListCache<T>.Store.Remove(resourcePath);
            _clearActions.Remove($"L:{typeof(T).FullName}:{resourcePath}");

#if UNITY_EDITOR
            Debug.Log($"[InGameDataManager] Unloaded: {resourcePath}");
#endif
        }

        /// <summary>
        /// Dictionary로 로드된 특정 경로의 캐시를 해제합니다.
        /// </summary>
        /// <typeparam name="TKey">로드 시 사용한 키 타입</typeparam>
        /// <typeparam name="TValue">로드 시 사용한 값 타입</typeparam>
        /// <param name="resourcePath">해제할 경로</param>
        public void UnloadDictionary<TKey, TValue>(string resourcePath)
        {
            DictCache<TKey, TValue>.Store.Remove(resourcePath);
            _clearActions.Remove($"D:{typeof(TKey).FullName}:{typeof(TValue).FullName}:{resourcePath}");

#if UNITY_EDITOR
            Debug.Log($"[InGameDataManager] Unloaded Dictionary: {resourcePath}");
#endif
        }

        /// <summary>
        /// 모든 캐시를 해제합니다.
        /// </summary>
        public void UnloadAll()
        {
            foreach (var action in _clearActions.Values)
                action();

            _clearActions.Clear();

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
