using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using Foundation.Assertions;
using Foundation.Collections.IndexableCollection;

namespace Foundation.Data.TextData;

public sealed class TextDataParameterCollection : DbParameterCollection, IList<TextDataParameter>
{
    private readonly IndexableCollection<TextDataParameter> _collection;
    private readonly ListIndex<TextDataParameter> _listIndex;
    private readonly UniqueIndex<string, TextDataParameter> _nameIndex;

    public TextDataParameterCollection()
    {
        _listIndex = new ListIndex<TextDataParameter>("List");
        _nameIndex = new UniqueIndex<string, TextDataParameter>(
            "Name",
            parameter => GetKeyResponse.Create(true, parameter.ParameterName),
            SortOrder.None);

        _collection = new IndexableCollection<TextDataParameter>(_listIndex);
        _collection.Indexes.Add(_nameIndex);
    }

    public override int Add(object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        Assert.IsTrue(value is TextDataParameter);

        var parameter = (TextDataParameter)value;
        _collection.Add(parameter);
        return _collection.Count - 1;
    }

    public TextDataParameter Add(TextDataParameter parameter)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        _collection.Add(parameter);
        return parameter;
    }

    public override void AddRange(Array values) => throw new NotImplementedException();

    public override void Clear() => _collection.Clear();

    [Pure]
    public override bool Contains(string value) => _nameIndex.ContainsKey(value);

    public override bool Contains(object value) => throw new NotImplementedException();

    public override void CopyTo(Array array, int index) => throw new NotImplementedException();

    public override int Count => _collection.Count;

    public override IEnumerator GetEnumerator() => throw new NotImplementedException();

    protected override DbParameter GetParameter(string parameterName) => throw new NotImplementedException();

    protected override DbParameter GetParameter(int index) => throw new NotImplementedException();

    public override int IndexOf(string parameterName) => throw new NotImplementedException();

    public override int IndexOf(object value) => throw new NotImplementedException();

    public override void Insert(int index, object value) => throw new NotImplementedException();

    public override bool IsFixedSize => throw new NotImplementedException();

    public override bool IsReadOnly => throw new NotImplementedException();

    public override bool IsSynchronized => throw new NotImplementedException();

    public override void Remove(object value) => throw new NotImplementedException();

    public override void RemoveAt(string parameterName) => throw new NotImplementedException();

    public override void RemoveAt(int index) => throw new NotImplementedException();

    protected override void SetParameter(string parameterName, DbParameter value) => throw new NotImplementedException();

    protected override void SetParameter(int index, DbParameter value) => throw new NotImplementedException();

    public override object SyncRoot => throw new NotImplementedException();

    public TResult? GetParameterValue<TResult>(string parameterName)
    {
        Assert.IsTrue(Contains(parameterName));

        var parameter = _nameIndex[parameterName];
        var value = parameter.Value;

        Assert.IsTrue(value is TResult);
        return (TResult?)value;
    }

    public bool TryGetValue(string parameterName, [MaybeNullWhen(false)] out TextDataParameter parameter) =>
        _nameIndex.TryGetValue(parameterName, out parameter);

    int IList<TextDataParameter>.IndexOf(TextDataParameter item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return _listIndex.IndexOf(item);
    }

    void IList<TextDataParameter>.Insert(int index, TextDataParameter item) => throw new NotImplementedException();

    void IList<TextDataParameter>.RemoveAt(int index)
    {
        var parameter = _listIndex[index];
        _collection.Remove(parameter);
    }

    TextDataParameter IList<TextDataParameter>.this[int index]
    {
        get => _listIndex[index];

        set => throw new NotSupportedException();
    }

    void ICollection<TextDataParameter>.Add(TextDataParameter item)
    {
        ArgumentNullException.ThrowIfNull(item);

        _collection.Add(item);
    }

    void ICollection<TextDataParameter>.Clear() => _collection.Clear();

    bool ICollection<TextDataParameter>.Contains(TextDataParameter item) => throw new NotImplementedException();

    void ICollection<TextDataParameter>.CopyTo(TextDataParameter[] array, int arrayIndex) => throw new NotImplementedException();

    int ICollection<TextDataParameter>.Count => _collection.Count;

    bool ICollection<TextDataParameter>.IsReadOnly => _collection.IsReadOnly;

    bool ICollection<TextDataParameter>.Remove(TextDataParameter item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return _collection.Remove(item);
    }

    IEnumerator<TextDataParameter> IEnumerable<TextDataParameter>.GetEnumerator() => _collection.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _collection.GetEnumerator();
}