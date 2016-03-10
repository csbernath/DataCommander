namespace DataCommander.Foundation.Collections
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ObjectPoolItem<T>
    {
        private readonly int key;
        private readonly T value;
        private readonly DateTime creationDate;

        public ObjectPoolItem(
            int key,
            T value,
            DateTime creationDate)
        {
            this.key = key;
            this.value = value;
            this.creationDate = creationDate;
        }

        public int Key => this.key;

        public T Value => this.value;

        public DateTime CreationDate => this.creationDate;
    }
}