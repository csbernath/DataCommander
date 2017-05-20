namespace DataCommander.Foundation.Collections.ObjectPool
{
    using System;
    using System.Threading;

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
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(pool != null);
#endif

            this._pool = pool;
            this._item = pool.CreateObject(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value => this._item.Value;

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this._pool.DestroyObject(this._item);
        }
    }
}