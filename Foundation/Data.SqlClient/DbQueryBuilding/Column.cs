namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class Column(string columnName, string sqlDataTypeName, bool isNullable)
{
    public readonly string ColumnName = columnName;
    public readonly string SqlDataTypeName = sqlDataTypeName;
    public readonly bool IsNullable = isNullable;
}