namespace DataCommander.Foundation.Configuration
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using DataCommander.Foundation.Collections;
    using DataCommander.Foundation.Collections.IndexableCollection;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConfigurationNodeCollection : ICollection<ConfigurationNode>
    {
        private readonly IndexableCollection<ConfigurationNode> _collection;
        private readonly ListIndex<ConfigurationNode> _listIndex;
        private readonly UniqueIndex<string, ConfigurationNode> _nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationNodeCollection()
        {
            this._listIndex = new ListIndex<ConfigurationNode>("List");
            this._nameIndex = new UniqueIndex<string, ConfigurationNode>("Name", node => GetKeyResponse.Create(true, node.Name), SortOrder.Ascending);

            this._collection = new IndexableCollection<ConfigurationNode>(this._listIndex);
            this._collection.Indexes.Add(this._nameIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigurationNode this[string name] => this._nameIndex[name];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ConfigurationNode this[int index] => this._listIndex[index];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out ConfigurationNode item)
        {
            return this._nameIndex.TryGetValue(name, out item);
        }

        #region ICollection<ConfigurationNode> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(ConfigurationNode item)
        {
#if CONTRACTS_FULL
            Contract.Assert(item != null);
#endif
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
        public bool Contains(ConfigurationNode item)
        {
            return this._nameIndex.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ConfigurationNode[] array, int arrayIndex)
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
        public bool IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ConfigurationNode item)
        {
            return this._collection.Remove(item);
        }

#endregion

#region IEnumerable<ConfigurationNode> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ConfigurationNode> GetEnumerator()
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

        internal void Insert(int index, ConfigurationNode item)
        {
            var where = this._collection.Indexes.Where(current => current != this._listIndex);
            this._listIndex.Insert(index, item);

            foreach (var current in where)
            {
                current.Add(item);
            }
        }
    }
}