using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SurvivalGame.Editor
{
    /// <summary>
    /// CharacterVisualRestructure 실행 후 Visual_ 내 SkinnedMeshRenderer에
    /// 프로젝트 내 .mat 파일을 할당합니다.
    /// Tools > Survival > Setup Character Materials 로 실행 후 이 파일을 삭제하세요.
    /// </summary>
    public static class CharacterMaterialSetup
    {
        private const string PathZombie   = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Zombie.prefab";
        private const string PathWolf     = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Wolf.prefab";
        private const string PathScene    = "Assets/Game_Survival/Scenes/MainScene.unity";

        private const string MatSurvivor  = "Assets/Game_Survival/Resources/Prefabs/Combat/Survivor.mat";
        private const string MatZombie    = "Assets/Game_Survival/Resources/Prefabs/Combat/Zombie.mat";
        private const string MatWolf      = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Wolf.mat";

        // zombieC 텍스처 (Wolf 전용)
        private const string TexZombieC   = "Assets/Game_Survival/Art/Textures/Characters/zombieC.png";

        [MenuItem("Tools/Survival/Setup Character Materials")]
        public static void Run()
        {
            // Wolf 마테리얼에 텍스처 설정 (비어있으므로)
            SetupWolfMaterial();

            var matSurvivor = AssetDatabase.LoadAssetAtPath<Material>(MatSurvivor);
            var matZombie   = AssetDatabase.LoadAssetAtPath<Material>(MatZombie);
            var matWolf     = AssetDatabase.LoadAssetAtPath<Material>(MatWolf);

            if (matSurvivor == null || matZombie == null || matWolf == null)
            {
                Debug.LogError("[MaterialSetup] 마테리얼 로드 실패 — 경로를 확인하세요.");
                return;
            }

            ApplyToPrefab(PathZombie, "Visual_Zombie", matZombie);
            ApplyToPrefab(PathWolf,   "Visual_Wolf",   matWolf);
            ApplyToScenePlayer(matSurvivor);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[MaterialSetup] 완료");
        }

        // ──────────────────────────────────────────────

        private static void SetupWolfMaterial()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(MatWolf);
            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TexZombieC);

            if (mat == null || tex == null)
            {
                Debug.LogWarning("[MaterialSetup] Wolf 마테리얼 또는 텍스처 로드 실패");
                return;
            }

            mat.SetTexture("_BaseMap", tex);
            mat.SetTexture("_MainTex", tex);
            EditorUtility.SetDirty(mat);
            Debug.Log("[MaterialSetup] Enemy_Wolf.mat 텍스처 설정 완료");
        }

        private static void ApplyToPrefab(string prefabPath, string visualName, Material mat)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);

            var visual = root.transform.Find(visualName);
            if (visual == null)
            {
                Debug.LogWarning($"[MaterialSetup] {prefabPath} 에서 {visualName} 을 찾지 못했습니다.");
                PrefabUtility.UnloadPrefabContents(root);
                return;
            }

            AssignMaterialToRenderers(visual.gameObject, mat);

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log($"[MaterialSetup] {prefabPath} 완료");
        }

        private static void ApplyToScenePlayer(Material mat)
        {
            var scene = EditorSceneManager.OpenScene(PathScene, OpenSceneMode.Additive);

            GameObject playerGo = null;
            foreach (var go in scene.GetRootGameObjects())
            {
                if (go.name == "Player") { playerGo = go; break; }
            }

            if (playerGo == null)
            {
                Debug.LogWarning("[MaterialSetup] 씬에서 Player를 찾지 못했습니다.");
                EditorSceneManager.CloseScene(scene, true);
                return;
            }

            var visual = playerGo.transform.Find("Visual_Player");
            if (visual == null)
            {
                Debug.LogWarning("[MaterialSetup] Visual_Player 를 찾지 못했습니다.");
                EditorSceneManager.CloseScene(scene, true);
                return;
            }

            AssignMaterialToRenderers(visual.gameObject, mat);

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
            Debug.Log("[MaterialSetup] 씬 Player 마테리얼 완료");
        }

        private static void AssignMaterialToRenderers(GameObject root, Material mat)
        {
            var renderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var smr in renderers)
            {
                var mats = new Material[smr.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = mat;
                smr.sharedMaterials = mats;
            }

            // MeshRenderer도 처리 (혹시 있을 경우)
            var meshRenderers = root.GetComponentsInChildren<MeshRenderer>(true);
            foreach (var mr in meshRenderers)
            {
                var mats = new Material[mr.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++)
                    mats[i] = mat;
                mr.sharedMaterials = mats;
            }
        }
    }
}
