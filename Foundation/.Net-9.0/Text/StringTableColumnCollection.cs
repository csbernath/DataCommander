using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Text;

public class StringTableColumnCollection : IList<StringTableColumn>
{
    private readonly List<StringTableColumn> _columns = [];

    public int IndexOf(StringTableColumn item)
    {
        return _columns.IndexOf(item);
    }

    void IList<StringTableColumn>.Insert(int index, StringTableColumn item)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    void IList<StringTableColumn>.RemoveAt(int index)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    public StringTableColumn this[int index]
    {
        get => _columns[index];

        set => throw new Exception("The method or operation is not implemented.");
    }

    internal void Add(StringTableColumn item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _columns.Add(item);
    }

    void ICollection<StringTableColumn>.Add(StringTableColumn item)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    void ICollection<StringTableColumn>.Clear()
    {
        throw new Exception("The method or operation is not implemented.");
    }

    public bool Contains(StringTableColumn item)
    {
        return _columns.Contains(item);
    }

    void ICollection<StringTableColumn>.CopyTo(StringTableColumn[] array, int arrayIndex)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>
    /// 
    /// </summary>
    public int Count => _columns.Count;

    /// <summary>
    /// 
    /// </summary>
    public bool IsReadOnly => throw new Exception("The method or operation is not implemented.");

    bool ICollection<StringTableColumn>.Remove(StringTableColumn item)
    {
        throw new Exception("The method or operation is not implemented.");
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerator<StringTableColumn> GetEnumerator()
    {
        return _columns.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}