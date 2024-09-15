using System;

namespace Foundation.Data.SqlEngine;

public class ColumnSchema(string columnName, Type dataType, string dataTypeName, int? columnSize, int? numericPrecision,
    int? numericScale, bool? allowDbNull)
{
    public readonly string ColumnName = columnName;
    public readonly Type DataType = dataType;
    public readonly string DataTypeName = dataTypeName;
    public readonly int? ColumnSize = columnSize;
    public readonly int? NumericPrecision = numericPrecision;
    public readonly int? NumericScale = numericScale;
    public readonly bool? AllowDBNull = allowDbNull;
}