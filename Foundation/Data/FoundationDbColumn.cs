using System;

namespace Foundation.Data
{
    public sealed class FoundationDbColumn
    {
        public readonly bool? AllowDbNull;
        public readonly string BaseColumnName;
        public readonly string BaseSchemaName;
        public readonly string BaseTableName;
        public readonly string ColumnName;
        public readonly int ColumnOrdinal;
        public readonly int ColumnSize;
        public readonly Type DataType;
        public readonly bool? IsAliased;
        public readonly bool? IsExpression;
        public readonly bool? IsKey;
        public readonly bool? IsIdentity;
        public readonly bool? IsLong;
        public readonly bool? IsUnique;
        public readonly int NonVersionedProviderType;
        public readonly short? NumericPrecision;
        public readonly short? NumericScale;
        public readonly int ProviderType;

        public FoundationDbColumn(bool? allowDbNull, string baseColumnName, string baseSchemaName, string baseTableName, string columnName, int columnOrdinal, int columnSize,
            Type dataType, bool? isAliased, bool? isExpression, bool? isKey, bool? isIdentity, bool? isLong, bool? isUnique, int nonVersionedProviderType, short? numericPrecision,
            short? numericScale, int providerType)
        {
            AllowDbNull = allowDbNull;
            BaseColumnName = baseColumnName;
            BaseSchemaName = baseSchemaName;
            BaseTableName = baseTableName;
            ColumnName = columnName;
            ColumnOrdinal = columnOrdinal;
            ColumnSize = columnSize;
            DataType = dataType;
            IsAliased = isAliased;
            IsExpression = isExpression;
            IsKey = isKey;
            IsIdentity = isIdentity;
            IsLong = isLong;
            IsUnique = isUnique;
            NonVersionedProviderType = nonVersionedProviderType;
            NumericPrecision = numericPrecision;
            NumericScale = numericScale;
            ProviderType = providerType;
        }
    }
}