using System;

namespace Foundation.Data.SqlClient.DbQueryBuilding;

public sealed class DbQueryResultField(string name, Type dataType, bool isNullable)
{
    public readonly string Name = name;
    public readonly Type DataType = dataType;
    public readonly bool IsNullable = isNullable;
}