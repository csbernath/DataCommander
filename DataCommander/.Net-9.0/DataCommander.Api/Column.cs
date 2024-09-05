namespace DataCommander.Api;

public class Column(string columnName, int columnId, string typeName, bool? isNullable, bool isComputed, int defaultObjectId)
{
    public readonly string ColumnName = columnName;
    public readonly int ColumnId = columnId;
    public readonly string TypeName = typeName;
    public readonly bool? IsNullable = isNullable;
    public readonly bool IsComputed = isComputed;
    public readonly int DefaultObjectId = defaultObjectId;
}