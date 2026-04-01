using System.IO;
using UnityEngine;

namespace Framework.Core.SaveSystem
{
    /// <summary>
    /// JSON 파일과 PlayerPrefs 두 가지 방식의 데이터 저장을 지원하는 정적 유틸리티입니다.
    /// <br/><br/>
    /// - JSON: 캐릭터, 인벤토리 등 복잡한 게임 데이터 저장. 현재는 로컬 파일로 저장하지만 서버 저장으로 전환 예정.
    /// - PlayerPrefs: 볼륨, 해상도 등 단순 설정값 저장.
    /// </summary>
    public static class SaveSystem
    {
        #region Fields

        private const string FileExtension = ".json";

        #endregion

        #region JSON

        // TODO: 현재 JSON 데이터는 로컬 파일(Application.persistentDataPath)에 저장됩니다.
        //       추후 서버 저장 방식으로 전환 시 IStorageProvider 인터페이스를 도입하여
        //       LocalStorageProvider / RemoteStorageProvider 로 교체하세요.

        /// <summary>
        /// 데이터를 JSON 파일로 로컬에 저장합니다.
        /// 저장 경로: <see cref="Application.persistentDataPath"/>/<paramref name="key"/>.json
        /// </summary>
        /// <typeparam name="T">저장할 데이터 타입</typeparam>
        /// <param name="key">파일명으로 사용될 고유 키</param>
        /// <param name="data">저장할 데이터 객체</param>
        public static void Save<T>(string key, T data)
        {
            string path = GetFilePath(key);
            string json = JsonUtility.ToJson(data, prettyPrint: true);
            File.WriteAllText(path, json);

#if UNITY_EDITOR
            Debug.Log($"[SaveSystem] Saved → {path}");
#endif
        }

        /// <summary>
        /// JSON 파일에서 데이터를 불러옵니다.
        /// 파일이 없으면 <paramref name="defaultValue"/>를 반환합니다.
        /// </summary>
        /// <typeparam name="T">불러올 데이터 타입</typeparam>
        /// <param name="key">저장 시 사용한 고유 키</param>
        /// <param name="defaultValue">파일이 없을 때 반환할 기본값</param>
        /// <returns>불러온 데이터 또는 기본값</returns>
        public static T Load<T>(string key, T defaultValue = default)
        {
            string path = GetFilePath(key);

            if (!File.Exists(path))
            {
#if UNITY_EDITOR
                Debug.Log($"[SaveSystem] File not found, returning default → {path}");
#endif
                return defaultValue;
            }

            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<T>(json);
        }

        /// <summary>
        /// 저장된 JSON 파일을 삭제합니다. 파일이 없으면 아무 작업도 하지 않습니다.
        /// </summary>
        /// <param name="key">삭제할 데이터의 고유 키</param>
        public static void Delete(string key)
        {
            string path = GetFilePath(key);

            if (!File.Exists(path))
            {
                return;
            }

            File.Delete(path);

#if UNITY_EDITOR
            Debug.Log($"[SaveSystem] Deleted → {path}");
#endif
        }

        /// <summary>
        /// 해당 키의 JSON 파일이 존재하는지 확인합니다.
        /// </summary>
        /// <param name="key">확인할 데이터의 고유 키</param>
        /// <returns>파일이 존재하면 true</returns>
        public static bool Exists(string key)
        {
            return File.Exists(GetFilePath(key));
        }

        #endregion

        #region PlayerPrefs

        /// <summary>
        /// PlayerPrefs에 값을 저장합니다.
        /// 지원 타입: <see cref="int"/>, <see cref="float"/>, <see cref="string"/>, <see cref="bool"/>
        /// </summary>
        /// <typeparam name="T">저장할 값의 타입</typeparam>
        /// <param name="key">PlayerPrefs 키</param>
        /// <param name="value">저장할 값</param>
        public static void SavePref<T>(string key, T value)
        {
            switch (value)
            {
                case int i:
                    PlayerPrefs.SetInt(key, i);
                    break;
                case float f:
                    PlayerPrefs.SetFloat(key, f);
                    break;
                case string s:
                    PlayerPrefs.SetString(key, s);
                    break;
                case bool b:
                    // bool은 PlayerPrefs에 int(0/1)로 저장
                    PlayerPrefs.SetInt(key, b ? 1 : 0);
                    break;
                default:
                    Debug.LogError($"[SaveSystem] SavePref: 지원하지 않는 타입 {typeof(T).Name}");
                    return;
            }

            PlayerPrefs.Save();
        }

        /// <summary>
        /// PlayerPrefs에서 값을 불러옵니다.
        /// 지원 타입: <see cref="int"/>, <see cref="float"/>, <see cref="string"/>, <see cref="bool"/>
        /// </summary>
        /// <typeparam name="T">불러올 값의 타입</typeparam>
        /// <param name="key">PlayerPrefs 키</param>
        /// <param name="defaultValue">키가 없을 때 반환할 기본값</param>
        /// <returns>저장된 값 또는 기본값</returns>
        public static T LoadPref<T>(string key, T defaultValue = default)
        {
            if (!PlayerPrefs.HasKey(key))
            {
                return defaultValue;
            }

            object result = defaultValue;

            if (typeof(T) == typeof(int))
            {
                result = PlayerPrefs.GetInt(key);
            }
            else if (typeof(T) == typeof(float))
            {
                result = PlayerPrefs.GetFloat(key);
            }
            else if (typeof(T) == typeof(string))
            {
                result = PlayerPrefs.GetString(key);
            }
            else if (typeof(T) == typeof(bool))
            {
                result = PlayerPrefs.GetInt(key) == 1;
            }
            else
            {
                Debug.LogError($"[SaveSystem] LoadPref: 지원하지 않는 타입 {typeof(T).Name}");
            }

            return (T)result;
        }

        /// <summary>
        /// PlayerPrefs에서 특정 키를 삭제합니다.
        /// </summary>
        /// <param name="key">삭제할 키</param>
        public static void DeletePref(string key)
        {
            PlayerPrefs.DeleteKey(key);
            PlayerPrefs.Save();
        }

        /// <summary>
        /// PlayerPrefs에 해당 키가 존재하는지 확인합니다.
        /// </summary>
        /// <param name="key">확인할 키</param>
        /// <returns>키가 존재하면 true</returns>
        public static bool ExistsPref(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        #endregion

        #region Private Methods

        private static string GetFilePath(string key)
        {
            return Path.Combine(Application.persistentDataPath, key + FileExtension);
        }

        #endregion
    }
}
