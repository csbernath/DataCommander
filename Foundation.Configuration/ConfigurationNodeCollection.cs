using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Configuration
{
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
            _listIndex = new ListIndex<ConfigurationNode>("List");
            _nameIndex = new UniqueIndex<string, ConfigurationNode>("Name", node => GetKeyResponse.Create(true, node.Name), SortOrder.Ascending);

            _collection = new IndexableCollection<ConfigurationNode>(_listIndex);
            _collection.Indexes.Add(_nameIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigurationNode this[string name] => _nameIndex[name];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ConfigurationNode this[int index] => _listIndex[index];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool TryGetValue(string name, out ConfigurationNode item)
        {
            return _nameIndex.TryGetValue(name, out item);
        }

        #region ICollection<ConfigurationNode> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add(ConfigurationNode item)
        {
            Assert.IsTrue(item != null);
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
        public bool Contains(ConfigurationNode item)
        {
            return _nameIndex.Contains(item);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo(ConfigurationNode[] array, int arrayIndex)
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
        public bool IsReadOnly => false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Remove(ConfigurationNode item)
        {
            return _collection.Remove(item);
        }

#endregion

#region IEnumerable<ConfigurationNode> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ConfigurationNode> GetEnumerator()
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

        internal void Insert(int index, ConfigurationNode item)
        {
            var where = _collection.Indexes.Where(current => current != _listIndex);
            _listIndex.Insert(index, item);

            foreach (var current in where)
            {
                current.Add(item);
            }
        }
    }
}