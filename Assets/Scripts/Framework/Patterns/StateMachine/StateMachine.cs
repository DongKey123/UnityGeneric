using System;
using UnityEngine;

namespace Framework.Patterns.StateMachine
{
    /// <summary>
    /// 제네릭 유한 상태 머신(FSM)입니다.
    /// Owner 객체와 현재 상태를 관리하며, 상태 전환 시 Enter / Exit를 자동으로 호출합니다.
    /// </summary>
    /// <typeparam name="T">상태가 제어할 Owner 타입</typeparam>
    public class StateMachine<T>
    {
        #region Events

        /// <summary>상태가 전환될 때 발생합니다. (이전 상태, 새 상태)</summary>
        public event Action<IState<T>, IState<T>> OnStateChanged;

        #endregion

        #region Fields

        private readonly T _owner;
        private IState<T> _currentState;

        #endregion

        #region Properties

        /// <summary>현재 활성화된 상태입니다.</summary>
        public IState<T> CurrentState => _currentState;

        #endregion

        #region Constructor

        /// <summary>
        /// StateMachine을 생성합니다.
        /// </summary>
        /// <param name="owner">상태가 제어할 Owner 객체</param>
        public StateMachine(T owner)
        {
            _owner = owner;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 초기 상태를 설정합니다. Enter()를 호출합니다.
        /// </summary>
        /// <param name="initialState">초기 상태</param>
        public void SetInitialState(IState<T> initialState)
        {
            _currentState = initialState;
            _currentState.Enter(_owner);

#if UNITY_EDITOR
            Debug.Log($"[StateMachine] Initial: {_currentState.GetType().Name}");
#endif
        }

        /// <summary>
        /// 현재 상태의 Update()를 호출합니다. 매 프레임 호출하세요.
        /// </summary>
        public void Update()
        {
            _currentState?.Update(_owner);
        }

        /// <summary>
        /// 새로운 상태로 전환합니다.
        /// 현재 상태의 Exit()를 호출한 후 새 상태의 Enter()를 호출합니다.
        /// 동일한 상태로 전환하면 무시합니다.
        /// </summary>
        /// <param name="newState">전환할 새 상태</param>
        public void ChangeState(IState<T> newState)
        {
            if (_currentState == newState)
            {
                return;
            }

            var prevState = _currentState;

            _currentState?.Exit(_owner);
            _currentState = newState;
            _currentState.Enter(_owner);

            OnStateChanged?.Invoke(prevState, _currentState);

#if UNITY_EDITOR
            Debug.Log($"[StateMachine] {prevState?.GetType().Name} → {_currentState.GetType().Name}");
#endif
        }

        /// <summary>
        /// 현재 상태가 지정한 타입인지 확인합니다.
        /// </summary>
        /// <typeparam name="TState">확인할 상태 타입</typeparam>
        /// <returns>현재 상태가 TState이면 true</returns>
        public bool IsState<TState>() where TState : IState<T>
        {
            return _currentState is TState;
        }

        #endregion
    }
}
