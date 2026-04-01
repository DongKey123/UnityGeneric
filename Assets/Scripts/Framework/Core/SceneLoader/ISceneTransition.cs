using System.Collections;

namespace Framework.Core.SceneLoader
{
    /// <summary>
    /// 씬 전환 효과 인터페이스입니다.
    /// 페이드, 슬라이드 등 커스텀 전환 효과를 구현할 때 사용합니다.
    /// </summary>
    public interface ISceneTransition
    {
        /// <summary>
        /// 씬 로딩 시작 전 실행되는 전환 효과입니다. (예: 화면 가리기, 페이드 아웃)
        /// </summary>
        /// <returns>전환 완료까지 대기하는 코루틴</returns>
        IEnumerator OnTransitionIn();

        /// <summary>
        /// 씬 로딩 완료 후 실행되는 전환 효과입니다. (예: 화면 열기, 페이드 인)
        /// </summary>
        /// <returns>전환 완료까지 대기하는 코루틴</returns>
        IEnumerator OnTransitionOut();
    }
}
