namespace Foundation.Data.DbQueryBuilding
{
    public sealed class Column
    {
        public readonly string ColumnName;
        public readonly string SqlDataTypeName;
        public readonly bool IsNullable;

        public Column(string columnName, string sqlDataTypeName, bool isNullable)
        {
            ColumnName = columnName;
            SqlDataTypeName = sqlDataTypeName;
            IsNullable = isNullable;
        }
    }
}