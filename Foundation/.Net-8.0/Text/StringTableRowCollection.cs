using System.Collections;
using System.Collections.Generic;
using Foundation.Collections;

namespace Foundation.Text;

public class StringTableRowCollection : IEnumerable<StringTableRow>
{
    private readonly SegmentedCollection<StringTableRow> _rows = new(64);

    internal StringTableRowCollection()
    {
    }

    public int Count => _rows.Count;
    public void Add(StringTableRow row) => _rows.Add(row);

    public IEnumerator<StringTableRow> GetEnumerator()
    {
        IEnumerable<StringTableRow> enumerable = _rows;
        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}