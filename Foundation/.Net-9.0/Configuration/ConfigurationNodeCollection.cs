using System.Collections;
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

    public IEnumerator<ConfigurationNode> GetEnumerator() => _collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();

    internal void Insert(int index, ConfigurationNode item)
    {
        IEnumerable<ICollectionIndex<ConfigurationNode>> where = _collection.Indexes.Where(current => current != _listIndex);
        _listIndex.Insert(index, item);

        foreach (ICollectionIndex<ConfigurationNode> current in where)
            current.Add(item);
    }
}