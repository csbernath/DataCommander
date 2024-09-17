namespace Foundation.Data.SqlEngine;

public class ColumnValueSelector(Column sourceColumn) : IValueSelector
{
    private readonly Column _sourceColumn = sourceColumn;

    public ColumnSchema ResultColumnSchema => _sourceColumn.ColumnSchema;

    public object Select(object[] row)
    {
        var value = row[_sourceColumn.ColumnIndex];
        return value;
    }
}