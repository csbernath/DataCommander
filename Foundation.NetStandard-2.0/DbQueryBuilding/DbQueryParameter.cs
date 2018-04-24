using System.Data;

namespace Foundation.DbQueryBuilding
{
    public sealed class DbQueryParameter
    {
        public readonly string Name;
        public readonly string DataType;
        public readonly SqlDbType SqlDbType;
        public readonly string CSharpDataType;
        public readonly bool IsNullable;
        public readonly string CSharpValue;

        public DbQueryParameter(string name, string dataType, SqlDbType sqlDbType, string cSharpDataType, bool isNullable, string cSharpValue)
        {
            Name = name;
            DataType = dataType;
            SqlDbType = sqlDbType;
            CSharpDataType = cSharpDataType;
            IsNullable = isNullable;
            CSharpValue = cSharpValue;
        }
    }
}