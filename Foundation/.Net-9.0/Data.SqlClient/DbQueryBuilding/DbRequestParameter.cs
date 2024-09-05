using System.Data;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DbRequestParameter(string name, string dataType, SqlDbType sqlDbType, int size, bool isNullable, string cSharpValue)
{
    public readonly string Name = name;
    public readonly string DataType = dataType;
    public readonly SqlDbType SqlDbType = sqlDbType;
    public readonly int Size = size;
    public readonly bool IsNullable = isNullable;
    public readonly string CSharpValue = cSharpValue;
}