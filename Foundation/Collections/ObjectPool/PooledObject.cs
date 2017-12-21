using System;
using System.Threading;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections.ObjectPool
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class PooledObject<T> : IDisposable
    {
        private readonly ObjectPool<T> _pool;
        private readonly ObjectPoolItem<T> _item;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pool"></param>
        public PooledObject(ObjectPool<T> pool)
        {
            FoundationContract.Requires<ArgumentNullException>(pool != null);

            _pool = pool;
            _item = pool.CreateObject(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value => _item.Value;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            _pool.DestroyObject(_item);
        }
    }
}