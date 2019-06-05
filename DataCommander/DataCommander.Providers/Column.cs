namespace DataCommander.Providers
{
    public class Column
    {
        public readonly string ColumnName;
        public readonly int ColumnId;
        public readonly bool HasDefault;
        public readonly bool IsNullable;
        public readonly bool HasAutomaticValue;

        public Column(string columnName, int columnId, bool hasDefault, bool isNullable, bool hasAutomaticValue)
        {
            ColumnName = columnName;
            ColumnId = columnId;
            HasDefault = hasDefault;
            IsNullable = isNullable;
            HasAutomaticValue = hasAutomaticValue;
        }
    }
}