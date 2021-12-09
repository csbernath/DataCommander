﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Configuration;

public sealed class ConfigurationNodeCollection : ICollection<ConfigurationNode>
{
    private readonly IndexableCollection<ConfigurationNode> _collection;
    private readonly ListIndex<ConfigurationNode> _listIndex;
    private readonly UniqueIndex<string, ConfigurationNode> _nameIndex;

    public ConfigurationNodeCollection()
    {
        _listIndex = new ListIndex<ConfigurationNode>("List");
        _nameIndex = new UniqueIndex<string, ConfigurationNode>("Name", node => GetKeyResponse.Create(true, node.Name), SortOrder.Ascending);

        _collection = new IndexableCollection<ConfigurationNode>(_listIndex);
        _collection.Indexes.Add(_nameIndex);
    }

    public ConfigurationNode this[string name] => _nameIndex[name];
    public ConfigurationNode this[int index] => _listIndex[index];
    public bool TryGetValue(string name, out ConfigurationNode item) => _nameIndex.TryGetValue(name, out item);

    #region ICollection<ConfigurationNode> Members

    public void Add(ConfigurationNode item)
    {
        Assert.IsTrue(item != null);
        _collection.Add(item);
    }

    public void Clear() => _collection.Clear();
    public bool Contains(ConfigurationNode item) => _nameIndex.Contains(item);
    public void CopyTo(ConfigurationNode[] array, int arrayIndex) => _collection.CopyTo(array, arrayIndex);
    public int Count => _collection.Count;
    public bool IsReadOnly => false;
    public bool Remove(ConfigurationNode item) => _collection.Remove(item);

    #endregion

    #region IEnumerable<ConfigurationNode> Members

    public IEnumerator<ConfigurationNode> GetEnumerator() => _collection.GetEnumerator();

    #endregion

    #region IEnumerable Members

    IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

    #endregion

    internal void Insert(int index, ConfigurationNode item)
    {
        var where = _collection.Indexes.Where(current => current != _listIndex);
        _listIndex.Insert(index, item);

        foreach (var current in where)
            current.Add(item);
    }
}