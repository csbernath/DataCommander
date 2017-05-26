using System;

namespace Foundation.Collections.ObjectPool2
{
    /// <summary>
    /// 
    /// </summary>
    public static class ObjectPoolExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectPool"></param>
        /// <param name="create"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static PooledObject<T> Get<T>(this ObjectPool<T> objectPool, Func<T> create)
        {
            return new PooledObject<T>(objectPool, create);
        }
    }
}