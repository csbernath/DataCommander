using System;

namespace Foundation.Data
{
    public sealed class OrmColumn
    {
        public readonly string ColumnName;
        public Type DataType;
        public readonly bool AllowDbNull;

        public OrmColumn(string columnName, Type dataType, bool allowDbNull)
        {
            ColumnName = columnName;
            DataType = dataType;
            AllowDbNull = allowDbNull;
        }
    }
}