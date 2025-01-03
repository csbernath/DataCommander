namespace DataCommander.Api;

public class Column
{
    public readonly string ColumnName;
    public readonly int ColumnId;
    public readonly string TypeName;
    public readonly bool? IsNullable;
    public readonly bool IsComputed;
    public readonly int DefaultObjectId;

    public Column(string columnName, int columnId, string typeName, bool? isNullable, bool isComputed, int defaultObjectId)
    {
        ColumnName = columnName;
        ColumnId = columnId;
        TypeName = typeName;
        IsNullable = isNullable;
        IsComputed = isComputed;
        DefaultObjectId = defaultObjectId;
    }
}