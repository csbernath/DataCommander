using System;

namespace Foundation.DbQueryBuilding
{
    public sealed class DbQueryResultField
    {
        public readonly string Name;
        public readonly Type DataType;
        public readonly bool IsNullable;

        public DbQueryResultField(string name, Type dataType, bool isNullable)
        {
            Name = name;
            DataType = dataType;
            IsNullable = isNullable;
        }
    }
}