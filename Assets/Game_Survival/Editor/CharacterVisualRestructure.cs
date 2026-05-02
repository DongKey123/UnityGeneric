using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using SurvivalGame.Player;

namespace SurvivalGame.Editor
{
    /// <summary>
    /// 캐릭터 비주얼 전체 셋업 툴입니다.
    /// Visual_ 교체 → 마테리얼 할당 → Animator 설정까지 한 번에 처리합니다.
    /// Tools > Survival > Setup All Character Visuals 로 실행하세요.
    /// FBX Reimport 후 매번 실행하면 됩니다.
    /// </summary>
    public static class CharacterVisualRestructure
    {
        private const string PathCharFbx    = "Assets/Game_Survival/Art/Models/Characters/characterMedium.fbx";
        private const string PathController = "Assets/Game_Survival/Art/Animations/Characters/Character.controller";
        private const string PathZombie     = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Zombie.prefab";
        private const string PathWolf       = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Wolf.prefab";
        private const string PathScene      = "Assets/Game_Survival/Scenes/MainScene.unity";

        private const string MatSurvivor    = "Assets/Game_Survival/Resources/Prefabs/Combat/Survivor.mat";
        private const string MatZombie      = "Assets/Game_Survival/Resources/Prefabs/Combat/Zombie.mat";
        private const string MatWolf        = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Wolf.mat";
        private const string TexZombieC     = "Assets/Game_Survival/Art/Textures/Characters/zombieC.png";

        [MenuItem("Tools/Survival/Setup All Character Visuals")]
        public static void Run()
        {
            var charPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PathCharFbx);
            var controller = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>(PathController);

            if (charPrefab == null || controller == null)
            {
                Debug.LogError("[CharacterSetup] FBX 또는 Controller 로드 실패");
                return;
            }

            var matSurvivor = AssetDatabase.LoadAssetAtPath<Material>(MatSurvivor);
            var matZombie   = AssetDatabase.LoadAssetAtPath<Material>(MatZombie);
            var matWolf     = LoadOrSetupWolfMaterial();

            if (matSurvivor == null || matZombie == null || matWolf == null)
            {
                Debug.LogError("[CharacterSetup] 마테리얼 로드 실패");
                return;
            }

            SetupPrefab(PathZombie, "Visual_Zombie", charPrefab, controller, matZombie);
            SetupPrefab(PathWolf,   "Visual_Wolf",   charPrefab, controller, matWolf);
            SetupScenePlayer(charPrefab, controller, matSurvivor);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[CharacterSetup] 전체 완료 — Zombie / Wolf / Player");
        }

        // ──────────────────────────────────────────────

        private static void SetupPrefab(string prefabPath, string visualName,
                                        GameObject charPrefab, RuntimeAnimatorController controller,
                                        Material mat)
        {
            var root = PrefabUtility.LoadPrefabContents(prefabPath);

            var old = root.transform.Find(visualName);
            if (old != null) Object.DestroyImmediate(old.gameObject);

            var visual = Object.Instantiate(charPrefab, root.transform);
            visual.name = visualName;
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            visual.transform.localScale    = Vector3.one;

            SetAnimator(visual, controller);
            ApplyMaterial(visual, mat);

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            PrefabUtility.UnloadPrefabContents(root);
            Debug.Log($"[CharacterSetup] {prefabPath} 완료");
        }

        private static void SetupScenePlayer(GameObject charPrefab,
                                             RuntimeAnimatorController controller,
                                             Material mat)
        {
            var scene = EditorSceneManager.OpenScene(PathScene, OpenSceneMode.Additive);

            GameObject playerGo = null;
            foreach (var go in scene.GetRootGameObjects())
            {
                if (go.name == "Player") { playerGo = go; break; }
            }

            if (playerGo == null)
            {
                Debug.LogWarning("[CharacterSetup] 씬에서 Player를 찾지 못했습니다.");
                EditorSceneManager.CloseScene(scene, true);
                return;
            }

            var oldVisual = playerGo.transform.Find("Visual_Player");
            if (oldVisual != null) Object.DestroyImmediate(oldVisual.gameObject);

            var rootMf = playerGo.GetComponent<MeshFilter>();
            var rootMr = playerGo.GetComponent<MeshRenderer>();
            if (rootMf != null) Object.DestroyImmediate(rootMf);
            if (rootMr != null) Object.DestroyImmediate(rootMr);

            var visual = Object.Instantiate(charPrefab, playerGo.transform);
            visual.name = "Visual_Player";
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            visual.transform.localScale    = Vector3.one;

            var anim = SetAnimator(visual, controller);
            ApplyMaterial(visual, mat);

            // PlayerController._animator 필드 연결
            var pc = playerGo.GetComponent<PlayerController>();
            if (pc != null)
            {
                var so   = new SerializedObject(pc);
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
            Debug.Log("[CharacterSetup] 씬 Player 완료");
        }

        private static Animator SetAnimator(GameObject visual, RuntimeAnimatorController controller)
        {
            // Animator는 Visual_ 루트가 아닌 FBX 내부 Root 본에 부착
            var rootBone = visual.transform.Find("Root");
            var target   = rootBone != null ? rootBone.gameObject : visual;

            // 기존 Animator 제거 후 새로 추가 (중복 방지)
            foreach (var old in visual.GetComponentsInChildren<Animator>(true))
                Object.DestroyImmediate(old);

            var anim = target.AddComponent<Animator>();
            anim.runtimeAnimatorController = controller;
            anim.applyRootMotion = false;
            return anim;
        }

        private static void ApplyMaterial(GameObject visual, Material mat)
        {
            foreach (var smr in visual.GetComponentsInChildren<SkinnedMeshRenderer>(true))
            {
                var mats = new Material[smr.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++) mats[i] = mat;
                smr.sharedMaterials = mats;
            }
            foreach (var mr in visual.GetComponentsInChildren<MeshRenderer>(true))
            {
                var mats = new Material[mr.sharedMaterials.Length];
                for (int i = 0; i < mats.Length; i++) mats[i] = mat;
                mr.sharedMaterials = mats;
            }
        }

        private static Material LoadOrSetupWolfMaterial()
        {
            var mat = AssetDatabase.LoadAssetAtPath<Material>(MatWolf);
            if (mat == null) return null;

            var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(TexZombieC);
            if (tex != null && mat.GetTexture("_BaseMap") == null)
            {
                mat.SetTexture("_BaseMap", tex);
                mat.SetTexture("_MainTex", tex);
                EditorUtility.SetDirty(mat);
            }
            return mat;
        }
    }
}
