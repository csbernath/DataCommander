namespace DataCommander.Foundation.Collections.ObjectPool2
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class PooledObject<T> : IDisposable
    {
        private readonly ObjectPool<T> _objectPool;
        private readonly T _object;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectPool"></param>
        /// <param name="create"></param>
        public PooledObject(ObjectPool<T> objectPool, Func<T> create)
        {
            _objectPool = objectPool;

            if (!objectPool.TryRemove(out _object))
                _object = create();
        }

        /// <summary>
        /// 
        /// </summary>
        public T Value => _object;

        void IDisposable.Dispose()
        {
            _objectPool.Add(_object);
        }
    }
}