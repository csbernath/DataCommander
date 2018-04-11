using System;

namespace Foundation.Data.SqlClient.Orm
{
    public class Field
    {
        public readonly string Name;
        public readonly Type DataType;
        public readonly bool IsNullable;

        public Field(string name, Type dataType, bool isNullable)
        {
            Name = name;
            DataType = dataType;
            IsNullable = isNullable;
        }
    }
}