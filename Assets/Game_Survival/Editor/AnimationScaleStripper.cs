using UnityEditor;
using UnityEngine;

namespace SurvivalGame.Editor
{
    /// <summary>
    /// Assets/Game_Survival/Art/Animations/ 경로의 FBX 애니메이션 임포트 시
    /// 스케일 커브를 자동으로 제거합니다.
    ///
    /// Kenney 애니메이션처럼 메시에 스케일 키프레임이 포함된 경우,
    /// Animator 재생 중 Visual 오브젝트의 스케일이 덮어써지는 문제를 방지합니다.
    /// </summary>
    public class AnimationScaleStripper : AssetPostprocessor
    {
        private void OnPostprocessAnimation(GameObject root, AnimationClip clip)
        {
            if (!assetPath.Contains("Game_Survival/Art/Animations")) return;
            StripScaleCurves(clip);
        }

        /// <summary>
        /// 이미 임포트된 Characters 폴더의 애니메이션 클립에서 스케일 커브를 일괄 제거합니다.
        /// Tools > Survival > Strip Animation Scale Curves 로 실행하세요.
        /// </summary>
        [MenuItem("Tools/Survival/Strip Animation Scale Curves")]
        public static void RunOnExistingClips()
        {
            string[] guids = AssetDatabase.FindAssets(
                "t:AnimationClip",
                new[] { "Assets/Game_Survival/Art/Animations" });

            int totalRemoved = 0;
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
                if (clip == null) continue;
                totalRemoved += StripScaleCurves(clip);
            }

            AssetDatabase.SaveAssets();
            Debug.Log($"[AnimationScaleStripper] 완료 — 총 {totalRemoved}개 스케일 커브 제거");
        }

        private static int StripScaleCurves(AnimationClip clip)
        {
            var bindings = AnimationUtility.GetCurveBindings(clip);
            int removed = 0;

            foreach (var binding in bindings)
            {
                if (binding.propertyName.StartsWith("m_LocalScale"))
                {
                    AnimationUtility.SetEditorCurve(clip, binding, null);
                    removed++;
                }
            }

            if (removed > 0)
                Debug.Log($"[AnimationScaleStripper] {clip.name} — 스케일 커브 {removed}개 제거");

            return removed;
        }
    }
}
