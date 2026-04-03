namespace Framework.UI
{
    /// <summary>
    /// 열릴 때 데이터를 전달받는 UI 패널 인터페이스입니다.
    /// 데이터가 필요한 패널에서 <see cref="UIPanel"/>과 함께 구현하세요.
    /// </summary>
    /// <typeparam name="TData">전달받을 데이터 타입</typeparam>
    public interface IInitializable<TData> where TData : class
    {
        /// <summary>
        /// UIManager.Open() 호출 시 패널이 열리기 전에 호출됩니다.
        /// </summary>
        /// <param name="data">전달받을 데이터. null이면 호출되지 않습니다.</param>
        void Initialize(TData data);
    }
}
