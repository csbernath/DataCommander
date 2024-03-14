using System;

namespace Foundation.Data.SqlEngine;

public class ColumnSchema
{
    public readonly string ColumnName;
    public readonly Type DataType;
    public readonly string DataTypeName;
    public readonly int? ColumnSize;
    public readonly int? NumericPrecision;
    public readonly int? NumericScale;
    public readonly bool? AllowDBNull;

    public ColumnSchema(string columnName, Type dataType, string dataTypeName, int? columnSize, int? numericPrecision,
        int? numericScale, bool? allowDbNull)
    {
        ColumnName = columnName;
        DataType = dataType;
        DataTypeName = dataTypeName;
        ColumnSize = columnSize;
        NumericPrecision = numericPrecision;
        NumericScale = numericScale;
        AllowDBNull = allowDbNull;
    }
}