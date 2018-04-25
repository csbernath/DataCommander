using System.Data;

namespace Foundation.DbQueryBuilding
{
    public sealed class DbQueryParameter
    {
        public readonly string Name;
        public readonly string DataType;
        public readonly SqlDbType SqlDbType;
        public readonly bool IsNullable;
        public readonly string CSharpValue;

        public DbQueryParameter(string name, string dataType, SqlDbType sqlDbType, bool isNullable, string cSharpValue)
        {
            Name = name;
            DataType = dataType;
            SqlDbType = sqlDbType;
            IsNullable = isNullable;
            CSharpValue = cSharpValue;
        }
    }
}