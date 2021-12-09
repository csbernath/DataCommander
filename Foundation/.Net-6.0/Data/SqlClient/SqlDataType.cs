using System.Data;

namespace Foundation.Data.SqlClient
{
    public sealed class SqlDataType
    {
        public SqlDbType SqlDbType;
        public readonly string SqlDataTypeName;
        public readonly string CSharpTypeName;

        public SqlDataType(SqlDbType sqlDbType, string sqlDataTypeName, string cSharpTypeName)
        {
            SqlDbType = sqlDbType;
            SqlDataTypeName = sqlDataTypeName;
            CSharpTypeName = cSharpTypeName;
        }
    }
}