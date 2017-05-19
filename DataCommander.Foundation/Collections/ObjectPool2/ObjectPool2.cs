namespace DataCommander.Foundation.Collections.ObjectPool2
{
    using DataCommander.Foundation.Collections.IndexableCollection;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ObjectPool<T>
    {
        private readonly NonUniqueIndex<int, T> _objects;

        /// <summary>
        /// 
        /// </summary>
        public ObjectPool()
        {
            _objects = new NonUniqueIndex<int, T>(null, @object => new GetKeyResponse<int>(true, @object.GetHashCode()), SortOrder.None);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        public void Add(T @object)
        {
            lock (_objects)
                _objects.Add(@object);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="object"></param>
        /// <returns></returns>
        public bool TryRemove(out T @object)
        {
            @object = default(T);
            var succeeded = false;
            lock (_objects)
            {
                if (_objects.Count > 0)
                    foreach (var currentObject in _objects)
                    {
                        @object = currentObject;
                        succeeded = true;
                        break;
                    }
            }
            return succeeded;
        }
    }
}