namespace Foundation.Data.SqlEngine;

public class ColumnValueSelector(Column sourceColumn) : IValueSelector
{
    public ColumnSchema ResultColumnSchema => sourceColumn.ColumnSchema;

    public object Select(object[] row)
    {
        var value = row[sourceColumn.ColumnIndex];
        return value;
    }
}