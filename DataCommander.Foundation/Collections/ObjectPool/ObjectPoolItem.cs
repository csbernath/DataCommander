namespace DataCommander.Foundation.Collections
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ObjectPoolItem<T>
    {
        private readonly Int32 key;
        private readonly T value;
        private readonly DateTime creationDate;

        public ObjectPoolItem(
            Int32 key,
            T value,
            DateTime creationDate)
        {
            this.key = key;
            this.value = value;
            this.creationDate = creationDate;
        }

        public Int32 Key
        {
            get
            {
                return this.key;
            }
        }

        public T Value
        {
            get
            {
                return this.value;
            }
        }

        public DateTime CreationDate
        {
            get
            {
                return this.creationDate;
            }
        }
    }
}