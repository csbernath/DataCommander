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
            Key = key;
            Value = value;
            CreationDate = creationDate;
        }

        public int Key { get; }

        public T Value { get; }

        public DateTime CreationDate { get; }
    }
}