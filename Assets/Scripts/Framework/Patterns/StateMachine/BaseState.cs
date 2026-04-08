namespace Framework.Patterns.StateMachine
{
    /// <summary>
    /// <see cref="IState{T}"/>의 추상 기반 클래스입니다.
    /// 필요한 메서드만 override하여 구현할 수 있습니다.
    /// </summary>
    /// <typeparam name="T">상태가 제어할 Owner 타입</typeparam>
    public abstract class BaseState<T> : IState<T>
    {
        /// <summary>상태에 진입할 때 한 번 호출됩니다.</summary>
        public virtual void Enter(T owner) { }

        /// <summary>상태가 활성화된 동안 매 프레임 호출됩니다.</summary>
        public virtual void Update(T owner) { }

        /// <summary>상태에서 벗어날 때 한 번 호출됩니다.</summary>
        public virtual void Exit(T owner) { }
    }
}
