namespace Foundation.Data.SqlClient.Orm
{
    public class Parameter
    {
        public readonly string Name;
        public readonly string DataType;
        public readonly bool IsNullable;

        public Parameter(string name, string dataType, bool isNullable)
        {
            Name = name;
            DataType = dataType;
            IsNullable = isNullable;
        }
    }
}