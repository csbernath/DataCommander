using System.Collections;
using System.Collections.Generic;
using Foundation.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Collections.IndexableCollection
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IndexCollection<T> : ICollection<ICollectionIndex<T>>
    {
        private readonly Dictionary<string, ICollectionIndex<T>> _dictionary = new Dictionary<string, ICollectionIndex<T>>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(ICollectionIndex<T> item)
        {
            Assert.IsTrue(item != null);

            _dictionary.Add(item.Name, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ICollectionIndex<T> this[string name] => _dictionary[name];

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            FoundationContract.Ensures(Count == 0);

            _dictionary.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ICollectionIndex<T> item)
        {
            //FoundationContract.Ensures(!Contract.Result<bool>() || this.Count > 0);

            return _dictionary.ContainsValue(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ICollectionIndex<T>[] array, int arrayIndex)
        {
            _dictionary.Values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => _dictionary.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ICollectionIndex<T> item)
        {
            bool succeeded;
            var contains = _dictionary.ContainsValue(item);

            if (contains)
            {
                succeeded = _dictionary.Remove(item.Name);
            }
            else
            {
                succeeded = false;
            }

            return succeeded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out ICollectionIndex<T> item)
        {
            return _dictionary.TryGetValue(name, out item);
        }

#region IEnumerable<ICollectionIndex<T>> Members

        /// <summary>
        /// 
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
    }
}