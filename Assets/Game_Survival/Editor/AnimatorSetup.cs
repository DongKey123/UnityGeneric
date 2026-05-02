using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace SurvivalGame.Editor
{
    /// <summary>
    /// Character.controller 를 생성합니다.
    /// Tools > Survival > Create Animator Controller 로 실행 후 이 파일을 삭제하세요.
    /// </summary>
    public static class AnimatorSetup
    {
        private const string OutPath     = "Assets/Game_Survival/Art/Animations/Characters/Character.controller";
        private const string IdleFbx     = "Assets/Game_Survival/Art/Animations/Characters/idle.fbx";
        private const string RunFbx      = "Assets/Game_Survival/Art/Animations/Characters/run.fbx";
        private const string JumpFbx     = "Assets/Game_Survival/Art/Animations/Characters/jump.fbx";

        [MenuItem("Tools/Survival/Create Animator Controller")]
        public static void Create()
        {
            var idleClip = LoadClip(IdleFbx, "Root|Idle");
            var runClip  = LoadClip(RunFbx,  "Root|Run");
            var jumpClip = LoadClip(JumpFbx, "Root|Jump");

            if (idleClip == null || runClip == null || jumpClip == null)
            {
                Debug.LogError("[AnimatorSetup] 클립 로드 실패. FBX 임포트 완료 후 다시 실행하세요.");
                return;
            }

            var controller = AnimatorController.CreateAnimatorControllerAtPath(OutPath);

            // 파라미터
            controller.AddParameter("Speed",     AnimatorControllerParameterType.Float);
            controller.AddParameter("IsJumping", AnimatorControllerParameterType.Bool);

            var sm = controller.layers[0].stateMachine;

            // 상태
            var idleState = sm.AddState("Idle");
            var runState  = sm.AddState("Run");
            var jumpState = sm.AddState("Jump");

            idleState.motion = idleClip;
            runState.motion  = runClip;
            jumpState.motion = jumpClip;

            sm.defaultState = idleState;

            // Idle → Run  (Speed > 0.1)
            var idleToRun = idleState.AddTransition(runState);
            idleToRun.AddCondition(AnimatorConditionMode.Greater, 0.1f, "Speed");
            idleToRun.hasExitTime = false;
            idleToRun.duration    = 0.15f;

            // Run → Idle  (Speed < 0.1)
            var runToIdle = runState.AddTransition(idleState);
            runToIdle.AddCondition(AnimatorConditionMode.Less, 0.1f, "Speed");
            runToIdle.hasExitTime = false;
            runToIdle.duration    = 0.15f;

            // Any → Jump  (IsJumping == true)
            var anyToJump = sm.AddAnyStateTransition(jumpState);
            anyToJump.AddCondition(AnimatorConditionMode.If, 0, "IsJumping");
            anyToJump.hasExitTime    = false;
            anyToJump.duration       = 0.05f;
            anyToJump.canTransitionToSelf = false;

            // Jump → Idle  (IsJumping == false, exit time 활용)
            var jumpToIdle = jumpState.AddTransition(idleState);
            jumpToIdle.AddCondition(AnimatorConditionMode.IfNot, 0, "IsJumping");
            jumpToIdle.hasExitTime = true;
            jumpToIdle.exitTime    = 0.9f;
            jumpToIdle.duration    = 0.15f;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[AnimatorSetup] 생성 완료: {OutPath}");
        }

        private static AnimationClip LoadClip(string fbxPath, string clipName)
        {
            foreach (var obj in AssetDatabase.LoadAllAssetsAtPath(fbxPath))
                if (obj is AnimationClip clip && clip.name == clipName)
                    return clip;
            return null;
        }
    }
}
