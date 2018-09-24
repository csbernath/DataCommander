using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Configuration
{
    [DebuggerTypeProxy(typeof(ConfigurationAttributeCollectionDebugger))]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class ConfigurationAttributeCollection : IList<ConfigurationAttribute>
    {
        private readonly IndexableCollection<ConfigurationAttribute> _collection;
        private readonly ListIndex<ConfigurationAttribute> _listIndex;
        private readonly UniqueIndex<string, ConfigurationAttribute> _nameIndex;

        public ConfigurationAttributeCollection()
        {
            _listIndex = new ListIndex<ConfigurationAttribute>("List");

            _nameIndex = new UniqueIndex<string, ConfigurationAttribute>(
                "NameIndex",
                attribute => GetKeyResponse.Create(true, attribute.Name),
                SortOrder.None);

            _collection = new IndexableCollection<ConfigurationAttribute>(_listIndex);
            _collection.Indexes.Add(_nameIndex);
        }

        public ConfigurationAttribute this[int index]
        {
            get
            {
                Assert.IsTrue(0 <= index && index < Count);
                return _listIndex[index];
            }

            set
            {
                var originalItem = _listIndex[index];
                ICollection<ConfigurationAttribute> collection = _nameIndex;
                collection.Remove(originalItem);
                _listIndex[index] = value;
                collection.Add(value);
            }
        }

        public ConfigurationAttribute this[string name]
        {
            get
            {
                Assert.IsInRange(ContainsKey(name));
                return _nameIndex[name];
            }
        }

        public string Name { get; set; }

        public void Add(string name, object value, string description)
        {
            Assert.IsValidOperation(!ContainsKey(name));

            var attribute = new ConfigurationAttribute(name, value, description);
            _collection.Add(attribute);
        }

        [Pure]
        public bool ContainsKey(string name) => _nameIndex.ContainsKey(name);
        public int IndexOf(ConfigurationAttribute item) => _listIndex.IndexOf(item);

        public void Insert(int index, ConfigurationAttribute item)
        {
            ICollection<ConfigurationAttribute> collection = _nameIndex;
            collection.Add(item);
            _listIndex.Insert(index, item);
        }

        public bool Remove(string name)
        {
            ConfigurationAttribute attribute;
            var contains = _nameIndex.TryGetValue(name, out attribute);
            bool succeeded;

            if (contains)
                succeeded = _collection.Remove(attribute);
            else
                succeeded = false;

            return succeeded;
        }

        public void RemoveAt(int index)
        {
            var item = _listIndex[index];
            _listIndex.RemoveAt(index);
            ICollection<ConfigurationAttribute> collection = _nameIndex;
            collection.Remove(item);
        }

        public bool TryGetValue(string name, out ConfigurationAttribute attribute) => _nameIndex.TryGetValue(name, out attribute);
        public bool TryGetAttributeValue<T>(string name, out T value) => TryGetAttributeValue(name, default(T), out value);

        public bool TryGetAttributeValue<T>(string name, T defaultValue, out T value)
        {
            var contains = _nameIndex.TryGetValue(name, out var attribute);
            value = contains ? attribute.GetValue<T>() : defaultValue;
            return contains;
        }

        public void SetAttributeValue(string name, object value)
        {
            var contains = _nameIndex.TryGetValue(name, out var attribute);
            if (contains)
                attribute.Value = value;
            else
            {
                attribute = new ConfigurationAttribute(name, value, null);
                _collection.Add(attribute);
            }
        }

        #region ICollection<Attribute> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(ConfigurationAttribute item)
        {
            _collection.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _collection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ConfigurationAttribute item)
        {
            return _collection.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ConfigurationAttribute[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => _collection.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => _collection.IsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ConfigurationAttribute item)
        {
            return _collection.Remove(item);
        }

        #endregion

        #region IEnumerable<Attribute> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ConfigurationAttribute> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion
    }
}