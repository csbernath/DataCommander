using System.Data;

namespace Foundation.Data.SqlClient;

public sealed class SqlDataType(SqlDbType sqlDbType, string sqlDataTypeName, string cSharpTypeName)
{
    public readonly SqlDbType SqlDbType = sqlDbType;
    public readonly string SqlDataTypeName = sqlDataTypeName;
    public readonly string CSharpTypeName = cSharpTypeName;
}