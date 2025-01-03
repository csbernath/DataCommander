using System;
using System.Collections;
using System.Collections.Generic;

namespace Foundation.Data.TextData;

/// <summary>
/// 
/// </summary>
public sealed class TextDataTableList : IList<TextDataTable>
{
    int IList<TextDataTable>.IndexOf(TextDataTable item) => throw new NotImplementedException();

    void IList<TextDataTable>.Insert(int index, TextDataTable item) => throw new NotImplementedException();

    void IList<TextDataTable>.RemoveAt(int index) => throw new NotImplementedException();

    TextDataTable IList<TextDataTable>.this[int index]
    {
        get => throw new NotImplementedException();

        set => throw new NotImplementedException();
    }

    void ICollection<TextDataTable>.Add(TextDataTable item) => throw new NotImplementedException();

    void ICollection<TextDataTable>.Clear() => throw new NotImplementedException();

    bool ICollection<TextDataTable>.Contains(TextDataTable item) => throw new NotImplementedException();

    void ICollection<TextDataTable>.CopyTo(TextDataTable[] array, int arrayIndex) => throw new NotImplementedException();

    int ICollection<TextDataTable>.Count => throw new NotImplementedException();

    bool ICollection<TextDataTable>.IsReadOnly => throw new NotImplementedException();

    bool ICollection<TextDataTable>.Remove(TextDataTable item) => throw new NotImplementedException();

    IEnumerator<TextDataTable> IEnumerable<TextDataTable>.GetEnumerator() => throw new NotImplementedException();

    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}