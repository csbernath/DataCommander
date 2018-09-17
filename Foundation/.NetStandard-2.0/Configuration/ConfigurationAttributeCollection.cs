using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;
using Foundation.Diagnostics.Contracts;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigurationAttribute this[string name]
        {
            get
            {
                Assert.IsInRange(ContainsKey(name));

                return _nameIndex[name];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        public void Add(string name, object value, string description)
        {
            Assert.IsValidOperation(!ContainsKey(name));

            var attribute = new ConfigurationAttribute(name, value, description);
            _collection.Add(attribute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Pure]
        public bool ContainsKey(string name)
        {
            return _nameIndex.ContainsKey(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ConfigurationAttribute item)
        {
            return _listIndex.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, ConfigurationAttribute item)
        {
            ICollection<ConfigurationAttribute> collection = _nameIndex;
            collection.Add(item);
            _listIndex.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            ConfigurationAttribute attribute;
            var contains = _nameIndex.TryGetValue(name, out attribute);
            bool succeeded;

            if (contains)
            {
                succeeded = _collection.Remove(attribute);
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
        /// <param name="index"></param>
        public void RemoveAt(int index)
        {
            var item = _listIndex[index];
            _listIndex.RemoveAt(index);
            ICollection<ConfigurationAttribute> collection = _nameIndex;
            collection.Remove(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out ConfigurationAttribute attribute)
        {
            return _nameIndex.TryGetValue(name, out attribute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetAttributeValue<T>(string name, out T value)
        {
            return TryGetAttributeValue(name, default(T), out value);
        }

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