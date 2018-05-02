using System.Data;

namespace Foundation.Data.DbQueryBuilding
{
    public sealed class DbRequestParameter
    {
        public readonly string Name;
        public readonly string DataType;
        public readonly SqlDbType SqlDbType;
        public readonly bool IsNullable;
        public readonly string CSharpValue;

        public DbRequestParameter(string name, string dataType, SqlDbType sqlDbType, bool isNullable, string cSharpValue)
        {
            Name = name;
            DataType = dataType;
            SqlDbType = sqlDbType;
            IsNullable = isNullable;
            CSharpValue = cSharpValue;
        }
    }
}