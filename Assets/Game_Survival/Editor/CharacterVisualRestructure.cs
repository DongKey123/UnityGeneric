using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SurvivalGame.Editor
{
    /// <summary>
    /// Visual_ 자식을 characterMedium.fbx 프리팹 인스턴스로 교체합니다.
    /// Tools > Survival > Restructure Character Visuals 로 실행 후 이 파일을 삭제하세요.
    /// </summary>
    public static class CharacterVisualRestructure
    {
        private const string PathCharFbx    = "Assets/Game_Survival/Art/Models/Characters/characterMedium.fbx";
        private const string PathController = "Assets/Game_Survival/Art/Animations/Characters/Character.controller";
        private const string PathZombie     = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Zombie.prefab";
        private const string PathWolf       = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Wolf.prefab";
        private const string PathScene      = "Assets/Game_Survival/Scenes/MainScene.unity";

        [MenuItem("Tools/Survival/Restructure Character Visuals")]
        public static void Run()
        {
            var charPrefab  = AssetDatabase.LoadAssetAtPath<GameObject>(PathCharFbx);
            var controller  = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PathController);

            if (charPrefab == null || controller == null)
            {
                Debug.LogError("[Restructure] FBX 또는 Controller 로드 실패");
                return;
            }

            RestructurePrefab(PathZombie, "Visual_Zombie", charPrefab, controller);
            RestructurePrefab(PathWolf,   "Visual_Wolf",   charPrefab, controller);
            RestructureScenePlayer(charPrefab, controller);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Restructure] 완료");
        }

        // ──────────────────────────────────────────────

        private static void RestructurePrefab(string prefabPath, string visualName,
                                              GameObject charPrefab, RuntimeAnimatorController controller)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);

            // 기존 Visual 제거
            var old = root.transform.Find(visualName);
            if (old != null) Object.DestroyImmediate(old.gameObject);

            // FBX 프리팹 인스턴스를 자식으로 추가
            var visual = (GameObject)PrefabUtility.InstantiatePrefab(charPrefab, root.transform);
            visual.name = visualName;
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            visual.transform.localScale    = Vector3.one;

            // Animator 연결
            var anim = visual.GetComponent<Animator>();
            if (anim == null) anim = visual.AddComponent<Animator>();
            anim.runtimeAnimatorController = controller;

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log($"[Restructure] {prefabPath} 완료");
        }

        private static void RestructureScenePlayer(GameObject charPrefab, RuntimeAnimatorController controller)
        {
            var scene = EditorSceneManager.OpenScene(PathScene, OpenSceneMode.Additive);

            GameObject playerGo = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Player") { playerGo = root; break; }
            }

            if (playerGo == null)
            {
                Debug.LogWarning("[Restructure] 씬에서 Player를 찾지 못했습니다.");
                EditorSceneManager.CloseScene(scene, true);
                return;
            }

            // 기존 Visual_Player 제거 및 루트 MeshFilter/MeshRenderer 제거
            var oldVisual = playerGo.transform.Find("Visual_Player");
            if (oldVisual != null) Object.DestroyImmediate(oldVisual.gameObject);

            var rootMf = playerGo.GetComponent<MeshFilter>();
            var rootMr = playerGo.GetComponent<MeshRenderer>();
            if (rootMf != null) Object.DestroyImmediate(rootMf);
            if (rootMr != null) Object.DestroyImmediate(rootMr);

            // FBX 프리팹 인스턴스 추가
            var visual = (GameObject)PrefabUtility.InstantiatePrefab(charPrefab, playerGo.transform);
            visual.name = "Visual_Player";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            visual.transform.localScale    = Vector3.one;

            // Animator 연결
            var anim = visual.GetComponent<Animator>();
            if (anim == null) anim = visual.AddComponent<Animator>();
            anim.runtimeAnimatorController = controller;

            // PlayerController._animator 필드 연결
            var pc = playerGo.GetComponent<PlayerController.PlayerController>();
            if (pc != null)
            {
                var so = new SerializedObject(pc);
                var prop = so.FindProperty("_animator");
                if (prop != null)
                {
                    prop.objectReferenceValue = anim;
                    so.ApplyModifiedProperties();
                }
            }

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
            Debug.Log("[Restructure] 씬 Player 완료");
        }
    }
}
