namespace DataCommander.Foundation.Collections
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public sealed class PooledObject<T> : IDisposable
    {
        private readonly ObjectPool<T> pool;
        private readonly ObjectPoolItem<T> item;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pool"></param>
        public PooledObject(ObjectPool<T> pool)
        {
            Contract.Requires<ArgumentNullException>(pool != null);

            this.pool = pool;
            this.item = pool.CreateObject(CancellationToken.None);
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value
        {
            get
            {
                return this.item.Value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.pool.DestroyObject(this.item);
        }
    }
}