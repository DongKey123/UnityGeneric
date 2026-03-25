namespace Framework.Core.ObjectPool
{
    /// <summary>
    /// 오브젝트 풀에서 관리되는 오브젝트가 구현해야 하는 인터페이스입니다.
    /// </summary>
    public interface IPoolable
    {
        /// <summary>
        /// 풀에서 꺼낼 때 호출됩니다.
        /// </summary>
        void OnSpawn();

        /// <summary>
        /// 풀로 반환할 때 호출됩니다.
        /// </summary>
        void OnDespawn();
    }
}
