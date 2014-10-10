namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public sealed class ConfigurationNodeCollection : ICollection<ConfigurationNode>
    {
        private IndexableCollection<ConfigurationNode> collection;

        private ListIndex<ConfigurationNode> listIndex;

        private UniqueIndex<String, ConfigurationNode> nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationNodeCollection()
        {
            this.listIndex = new ListIndex<ConfigurationNode>("List");
            this.nameIndex = new UniqueIndex<String, ConfigurationNode>("Name", node => GetKeyResponse.Create(true, node.Name), SortOrder.Ascending);

            this.collection = new IndexableCollection<ConfigurationNode>(this.listIndex);
            this.collection.Indexes.Add(this.nameIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigurationNode this[String name]
        {
            get
            {
                return this.nameIndex[name];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ConfigurationNode this[Int32 index]
        {
            get
            {
                return this.listIndex[index];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean TryGetValue(String name, out ConfigurationNode item)
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
        public Boolean Contains(ConfigurationNode item)
        {
            return this.nameIndex.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ConfigurationNode[] array, Int32 arrayIndex)
        {
            this.collection.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.collection.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove(ConfigurationNode item)
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

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        internal void Insert(Int32 index, ConfigurationNode item)
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