namespace Framework.Patterns.StateMachine
{
    /// <summary>
    /// StateMachine이 관리하는 상태의 인터페이스입니다.
    /// </summary>
    /// <typeparam name="T">상태가 제어할 Owner 타입</typeparam>
    public interface IState<T>
    {
        /// <summary>상태에 진입할 때 한 번 호출됩니다.</summary>
        void Enter(T owner);

        /// <summary>상태가 활성화된 동안 매 프레임 호출됩니다.</summary>
        void Update(T owner);

        /// <summary>상태에서 벗어날 때 한 번 호출됩니다.</summary>
        void Exit(T owner);
    }
}
