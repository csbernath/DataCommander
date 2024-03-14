namespace Foundation.Data.SqlEngine;

public interface IValueSelector
{
    ColumnSchema ResultColumnSchema { get; }
    object Select(object[] row);
}