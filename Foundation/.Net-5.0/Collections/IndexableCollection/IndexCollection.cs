using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Collections.IndexableCollection
{
    public class IndexCollection<T> : ICollection<ICollectionIndex<T>>
    {
        private readonly Dictionary<string, ICollectionIndex<T>> _dictionary = new Dictionary<string, ICollectionIndex<T>>();

        public ICollectionIndex<T> this[string name] => _dictionary[name];

        public void Add(ICollectionIndex<T> item)
        {
            Assert.IsTrue(item != null);
            _dictionary.Add(item.Name, item);
        }

        public void Clear() => _dictionary.Clear();
        public bool Contains(ICollectionIndex<T> item) => _dictionary.ContainsValue(item);
        public void CopyTo(ICollectionIndex<T>[] array, int arrayIndex) => _dictionary.Values.CopyTo(array, arrayIndex);
        public int Count => _dictionary.Count;
        public bool IsReadOnly => false;

        public bool Remove(ICollectionIndex<T> item)
        {
            bool succeeded;
            var contains = _dictionary.ContainsValue(item);
            succeeded = contains && _dictionary.Remove(item.Name);
            return succeeded;
        }

        #region IEnumerable<ICollectionIndex<T>> Members

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ICollectionIndex<T>> GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _dictionary.Values.GetEnumerator();
        }

        #endregion

        public bool TryGetValue(string name, out ICollectionIndex<T> item) => _dictionary.TryGetValue(name, out item);
    }
}