using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Foundation.Data.SqlEngine;

public class ColumnCollection : IReadOnlyList<Column>
{
    private readonly Column[] _columns;
    private readonly ILookup<string, Column> _columnsByColumnName;

    public ColumnCollection(IEnumerable<ColumnSchema> columnSchemas)
    {
        _columns = columnSchemas
            .Select((columnSchema, index) => new Column(index, columnSchema))
            .ToArray();
        _columnsByColumnName = _columns
            .ToLookup(c => c.ColumnSchema.ColumnName);
    }

    public int Count => _columns.Length;
    public Column this[int columnIndex] => _columns[columnIndex];
    public Column this[string columnName] => GetColumn(columnName);
    
    public IEnumerator<Column> GetEnumerator()
    {
        IEnumerable<Column> enumerable = (IEnumerable<Column>)_columns;
        return enumerable.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    private Column GetColumn(string columnName)
    {
        Column result = null;
        int count = 0;

        foreach (Column column in _columnsByColumnName[columnName])
        {
            ++count;
            if (count == 1)
            {
                result = column;
            }
            else
            {
                break;
            }
        }

        if (count > 1)
        {
            throw new InvalidOperationException(
                $"The column '{columnName}' was specified multiple times.");
        }

        return result;
    }
}