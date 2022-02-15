using System.Data;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DbRequestParameter
{
    public readonly string Name;
    public readonly string DataType;
    public readonly SqlDbType SqlDbType;
    public readonly int Size;
    public readonly bool IsNullable;
    public readonly string CSharpValue;

    public DbRequestParameter(string name, string dataType, SqlDbType sqlDbType, int size, bool isNullable, string cSharpValue)
    {
        Name = name;
        DataType = dataType;
        SqlDbType = sqlDbType;
        Size = size;
        IsNullable = isNullable;
        CSharpValue = cSharpValue;
    }
}