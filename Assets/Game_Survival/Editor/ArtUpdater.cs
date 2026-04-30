using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace SurvivalGame.Editor
{
    /// <summary>
    /// Kenney FBX 메시/텍스처를 프리팹 및 씬 오브젝트에 연결합니다.
    /// Tools > Survival > Update Art Assets 메뉴로 실행하세요.
    /// </summary>
    public static class ArtUpdater
    {
        private const string PathTreeFbx      = "Assets/Game_Survival/Art/Models/Environment/tree.fbx";
        private const string PathRockFbx      = "Assets/Game_Survival/Art/Models/Environment/rock-a.fbx";
        private const string PathCharFbx      = "Assets/Game_Survival/Art/Models/Characters/characterMedium.fbx";
        private const string PathZombieTex    = "Assets/Game_Survival/Art/Textures/Characters/zombieA.png";
        private const string PathSurvivorTex  = "Assets/Game_Survival/Art/Textures/Characters/survivorMaleB.png";
        private const string PathPlayerTex    = "Assets/Game_Survival/Art/Textures/Characters/survivorFemaleA.png";

        private const string PathResourceTree = "Assets/Game_Survival/Resources/Prefabs/Farming/Resource_Tree.prefab";
        private const string PathResourceRock = "Assets/Game_Survival/Resources/Prefabs/Farming/Resource_Rock.prefab";
        private const string PathEnemyZombie  = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Zombie.prefab";
        private const string PathEnemyWolf    = "Assets/Game_Survival/Resources/Prefabs/Combat/Enemy_Wolf.prefab";
        private const string PathScene        = "Assets/Game_Survival/Scenes/MainScene.unity";

        [MenuItem("Tools/Survival/Update Art Assets")]
        public static void UpdateAll()
        {
            var treeMesh     = LoadMesh(PathTreeFbx);
            var rockMesh     = LoadMesh(PathRockFbx);
            var charMesh     = LoadMesh(PathCharFbx);
            var zombieMat    = GetOrCreateMaterial("Zombie",    PathZombieTex);
            var survivorMat  = GetOrCreateMaterial("Survivor",  PathSurvivorTex);
            var playerMat    = GetOrCreateMaterial("Player",    PathPlayerTex);

            if (treeMesh == null || rockMesh == null || charMesh == null)
            {
                Debug.LogError("[ArtUpdater] FBX 메시 로드 실패. Art 폴더 경로를 확인하세요.");
                return;
            }

            UpdatePrefab(PathResourceTree, "Visual_Tree",   treeMesh,    null);
            UpdatePrefab(PathResourceRock, "Visual_Rock",   rockMesh,    null);
            UpdatePrefab(PathEnemyZombie,  "Visual_Zombie", charMesh,    zombieMat);
            UpdatePrefab(PathEnemyWolf,    "Visual_Wolf",   charMesh,    survivorMat);
            UpdateScenePlayer(charMesh, playerMat);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[ArtUpdater] 완료 — 프리팹 4개 + 씬 Player 업데이트");
        }

        // ──────────────────────────────────────────────
        // Helpers
        // ──────────────────────────────────────────────

        private static Mesh LoadMesh(string fbxPath)
        {
            var objects = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            foreach (var obj in objects)
                if (obj is Mesh mesh)
                    return mesh;
            return null;
        }

        private static Material GetOrCreateMaterial(string matName, string texturePath)
        {
            string matPath = $"Assets/Game_Survival/Resources/Prefabs/Combat/{matName}.mat";

            var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);
            if (mat != null)
            {
                // 텍스처만 교체
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
                if (tex != null) mat.mainTexture = tex;
                EditorUtility.SetDirty(mat);
                return mat;
            }

            // 새로 생성
            mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            var newTex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
            if (newTex != null) mat.mainTexture = newTex;
            AssetDatabase.CreateAsset(mat, matPath);
            return mat;
        }

        private static void UpdatePrefab(string prefabPath, string visualName, Mesh mesh, Material mat)
        {
            var prefabRoot = PrefabUtility.LoadPrefabContents(prefabPath);
            if (prefabRoot == null)
            {
                Debug.LogWarning($"[ArtUpdater] 프리팹 로드 실패: {prefabPath}");
                return;
            }

            var visual = prefabRoot.transform.Find(visualName);
            if (visual == null)
            {
                // 자식 전체 검색
                foreach (Transform child in prefabRoot.GetComponentsInChildren<Transform>())
                    if (child.name == visualName) { visual = child; break; }
            }

            if (visual == null)
            {
                Debug.LogWarning($"[ArtUpdater] Visual 오브젝트 없음: {visualName} in {prefabPath}");
                PrefabUtility.UnloadPrefabContents(prefabRoot);
                return;
            }

            var mf = visual.GetComponent<MeshFilter>();
            if (mf == null) mf = visual.gameObject.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            if (mat != null)
            {
                var mr = visual.GetComponent<MeshRenderer>();
                if (mr == null) mr = visual.gameObject.AddComponent<MeshRenderer>();
                mr.sharedMaterial = mat;
            }

            PrefabUtility.SaveAsPrefabAsset(prefabRoot, prefabPath);
            PrefabUtility.UnloadPrefabContents(prefabRoot);
            Debug.Log($"[ArtUpdater] 업데이트: {prefabPath}");
        }

        private static void UpdateScenePlayer(Mesh mesh, Material mat)
        {
            var scene = EditorSceneManager.OpenScene(PathScene, OpenSceneMode.Additive);

            GameObject playerGo = null;
            foreach (var root in scene.GetRootGameObjects())
            {
                if (root.name == "Player") { playerGo = root; break; }
                var found = root.transform.Find("Player");
                if (found != null) { playerGo = found.gameObject; break; }
            }

            if (playerGo == null)
            {
                Debug.LogWarning("[ArtUpdater] 씬에서 Player를 찾지 못했습니다.");
                EditorSceneManager.CloseScene(scene, true);
                return;
            }

            // 루트에 직접 붙은 MeshFilter/MeshRenderer 제거 (콜라이더 스케일 오염 방지)
            var rootMf = playerGo.GetComponent<MeshFilter>();
            var rootMr = playerGo.GetComponent<MeshRenderer>();
            if (rootMf != null) Object.DestroyImmediate(rootMf);
            if (rootMr != null) Object.DestroyImmediate(rootMr);

            // Visual_Player 자식 — 없으면 생성
            var visualT = playerGo.transform.Find("Visual_Player");
            GameObject visual;
            if (visualT == null)
            {
                visual = new GameObject("Visual_Player");
                visual.transform.SetParent(playerGo.transform, false);
            }
            else
            {
                visual = visualT.gameObject;
            }

            // 스케일 50x, 회전 X -90° (FBX 좌표계 보정), 위치 (0,0,0)
            visual.transform.localPosition = Vector3.zero;
            visual.transform.localRotation = Quaternion.Euler(-90f, 0f, 0f);
            visual.transform.localScale    = new Vector3(50f, 50f, 50f);

            var mf = visual.GetComponent<MeshFilter>();
            if (mf == null) mf = visual.AddComponent<MeshFilter>();
            mf.sharedMesh = mesh;

            var mr = visual.GetComponent<MeshRenderer>();
            if (mr == null) mr = visual.AddComponent<MeshRenderer>();
            if (mat != null) mr.sharedMaterial = mat;

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
            EditorSceneManager.CloseScene(scene, true);
            Debug.Log("[ArtUpdater] 씬 Player 업데이트 완료 (Visual_Player 자식 생성)");
        }
    }
}
