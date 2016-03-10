namespace DataCommander.Foundation.Configuration
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConfigurationNodeCollection : ICollection<ConfigurationNode>
    {
        private readonly IndexableCollection<ConfigurationNode> collection;
        private readonly ListIndex<ConfigurationNode> listIndex;
        private readonly UniqueIndex<string, ConfigurationNode> nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationNodeCollection()
        {
            this.listIndex = new ListIndex<ConfigurationNode>("List");
            this.nameIndex = new UniqueIndex<string, ConfigurationNode>("Name", node => GetKeyResponse.Create(true, node.Name), SortOrder.Ascending);

            this.collection = new IndexableCollection<ConfigurationNode>(this.listIndex);
            this.collection.Indexes.Add(this.nameIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigurationNode this[string name] => this.nameIndex[name];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ConfigurationNode this[int index] => this.listIndex[index];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out ConfigurationNode item)
        {
            return this.nameIndex.TryGetValue(name, out item);
        }

        #region ICollection<ConfigurationNode> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(ConfigurationNode item)
        {
            Contract.Assert(item != null);
            this.collection.Add(item);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.collection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(ConfigurationNode item)
        {
            return this.nameIndex.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ConfigurationNode[] array, int arrayIndex)
        {
            this.collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public int Count => this.collection.Count;

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
            return this.collection.Remove(item);
        }

        #endregion

        #region IEnumerable<ConfigurationNode> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ConfigurationNode> GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        internal void Insert(int index, ConfigurationNode item)
        {
            IEnumerable<ICollectionIndex<ConfigurationNode>> where = this.collection.Indexes.Where(current => current != this.listIndex);
            this.listIndex.Insert(index, item);

            foreach (ICollectionIndex<ConfigurationNode> current in where)
            {
                current.Add(item);
            }
        }
    }
}