namespace Foundation.Collections.ObjectPool
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPoolableObjectFactory<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        T CreateObject();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void InitializeObject(T value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        void DestroyObject(T value);
    }
}