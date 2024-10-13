using System;

namespace Foundation.Data;

public sealed class FoundationDbColumn(
    bool? allowDbNull,
    string? baseColumnName,
    string? baseSchemaName,
    string? baseTableName,
    string? columnName,
    int columnOrdinal,
    int columnSize,
    Type? dataType,
    bool? isAliased,
    bool? isExpression,
    bool? isKey,
    bool? isIdentity,
    bool? isLong,
    bool? isUnique,
    int nonVersionedProviderType,
    short? numericPrecision,
    short? numericScale,
    int providerType)
{
    public readonly bool? AllowDbNull = allowDbNull;
    public readonly string? BaseColumnName = baseColumnName;
    public readonly string? BaseSchemaName = baseSchemaName;
    public readonly string? BaseTableName = baseTableName;
    public readonly string? ColumnName = columnName;
    public readonly int ColumnOrdinal = columnOrdinal;
    public readonly int ColumnSize = columnSize;
    public readonly Type? DataType = dataType;
    public readonly bool? IsAliased = isAliased;
    public readonly bool? IsExpression = isExpression;
    public readonly bool? IsKey = isKey;
    public readonly bool? IsIdentity = isIdentity;
    public readonly bool? IsLong = isLong;
    public readonly bool? IsUnique = isUnique;
    public readonly int NonVersionedProviderType = nonVersionedProviderType;
    public readonly short? NumericPrecision = numericPrecision;
    public readonly short? NumericScale = numericScale;
    public readonly int ProviderType = providerType;
}