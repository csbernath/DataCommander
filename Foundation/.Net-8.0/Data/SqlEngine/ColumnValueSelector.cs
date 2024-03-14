namespace Foundation.Data.SqlEngine;

public class ColumnValueSelector : IValueSelector
{
    private readonly Column _sourceColumn;

    public ColumnValueSelector(Column sourceColumn)
    {
        _sourceColumn = sourceColumn;
    }

    public ColumnSchema ResultColumnSchema => _sourceColumn.ColumnSchema;

    public object Select(object[] row)
    {
        var value = row[_sourceColumn.ColumnIndex];
        return value;
    }
}