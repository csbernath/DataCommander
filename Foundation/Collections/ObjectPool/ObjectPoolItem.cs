using System;

namespace Foundation.Collections.ObjectPool
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ObjectPoolItem<T>
    {
        public ObjectPoolItem(
            int key,
            T value,
            DateTime creationDate)
        {
            this.Key = key;
            this.Value = value;
            this.CreationDate = creationDate;
        }

        public int Key { get; }

        public T Value { get; }

        public DateTime CreationDate { get; }
    }
}