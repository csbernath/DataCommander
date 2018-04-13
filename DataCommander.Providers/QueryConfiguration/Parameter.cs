using System.Data;

namespace DataCommander.Providers.QueryConfiguration
{
    public class Parameter
    {
        public readonly string Name;
        public readonly string DataType;
        public readonly SqlDbType? SqlDbType;
        public readonly string CSharpDataType;
        public readonly bool IsNullable;
        public readonly string CSharpValue;
        public readonly string Value;

        public Parameter(string name, string dataType, SqlDbType? sqlDbType, string cSharpDataType, bool isNullable, string cSharpValue, string value)
        {
            Name = name;
            DataType = dataType;
            SqlDbType = sqlDbType;
            CSharpDataType = cSharpDataType;
            IsNullable = isNullable;
            CSharpValue = cSharpValue;
            Value = value;
        }
    }
}