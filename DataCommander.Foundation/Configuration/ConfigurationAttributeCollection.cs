namespace DataCommander.Foundation.Configuration
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Collections;
    using DataCommander.Foundation.Collections.IndexableCollection;

    /// <summary>
    /// 
    /// </summary>
    [DebuggerTypeProxy(typeof (ConfigurationAttributeCollectionDebugger))]
    [DebuggerDisplay("Count = {" + nameof(Count) + "}")]
    public class ConfigurationAttributeCollection : IList<ConfigurationAttribute>
    {
        private readonly IndexableCollection<ConfigurationAttribute> _collection;
        private readonly ListIndex<ConfigurationAttribute> _listIndex;
        private readonly UniqueIndex<string, ConfigurationAttribute> _nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationAttributeCollection()
        {
            this._listIndex = new ListIndex<ConfigurationAttribute>("List");

            this._nameIndex = new UniqueIndex<string, ConfigurationAttribute>(
                "NameIndex",
                attribute => GetKeyResponse.Create(true, attribute.Name),
                SortOrder.None);

            this._collection = new IndexableCollection<ConfigurationAttribute>(this._listIndex);
            this._collection.Indexes.Add(this._nameIndex);
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
#if CONTRACTS_FULL
                Contract.Assert(0 <= index && index < this.Count);
#endif

                return this._listIndex[index];
            }

            set
            {
                var originalItem = this._listIndex[index];
                ICollection<ConfigurationAttribute> collection = this._nameIndex;
                collection.Remove(originalItem);
                this._listIndex[index] = value;
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
#if CONTRACTS_FULL
                Contract.Requires(this.ContainsKey(name));
#endif

                return this._nameIndex[name];
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
#if CONTRACTS_FULL
            Contract.Requires(!this.ContainsKey(name));
#endif
            var attribute = new ConfigurationAttribute(name, value, description);
            this._collection.Add(attribute);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Pure]
        public bool ContainsKey(string name)
        {
            return this._nameIndex.ContainsKey(name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public int IndexOf(ConfigurationAttribute item)
        {
            return this._listIndex.IndexOf(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, ConfigurationAttribute item)
        {
            ICollection<ConfigurationAttribute> collection = this._nameIndex;
            collection.Add(item);
            this._listIndex.Insert(index, item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Remove(string name)
        {
            ConfigurationAttribute attribute;
            var contains = this._nameIndex.TryGetValue(name, out attribute);
            bool succeeded;

            if (contains)
            {
                succeeded = this._collection.Remove(attribute);
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
            var item = this._listIndex[index];
            this._listIndex.RemoveAt(index);
            ICollection<ConfigurationAttribute> collection = this._nameIndex;
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
            return this._nameIndex.TryGetValue(name, out attribute);
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
            return this.TryGetAttributeValue(name, default(T), out value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool TryGetAttributeValue<T>(string name, T defaultValue, out T value)
        {
            ConfigurationAttribute attribute;
            var contains = this._nameIndex.TryGetValue(name, out attribute);

            if (contains)
            {
                value = attribute.GetValue<T>();
            }
            else
            {
                value = defaultValue;
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAttributeValue(string name, object value)
        {
            ConfigurationAttribute attribute;
            var contains = this._nameIndex.TryGetValue(name, out attribute);

            if (contains)
            {
                attribute.Value = value;
            }
            else
            {
                attribute = new ConfigurationAttribute(name, value, null);
                this._collection.Add(attribute);
            }
        }

#region ICollection<Attribute> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(ConfigurationAttribute item)
        {
            this._collection.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this._collection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ConfigurationAttribute item)
        {
            return this._collection.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ConfigurationAttribute[] array, int arrayIndex)
        {
            this._collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this._collection.Count;

        /// <summary>
        /// 
        /// </summary>
        public bool IsReadOnly => this._collection.IsReadOnly;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ConfigurationAttribute item)
        {
            return this._collection.Remove(item);
        }

#endregion

#region IEnumerable<Attribute> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ConfigurationAttribute> GetEnumerator()
        {
            return this._collection.GetEnumerator();
        }

#endregion

#region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._collection.GetEnumerator();
        }

#endregion
    }
}