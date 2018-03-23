using System;
using System.Data;
using System.Data.Common;
using Foundation.Diagnostics.Assertions;

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

    public static class FoundationDbColumnFactory
    {
        public static FoundationDbColumn Create(DataRow schemaTableRow)
        {
            Assert.IsNotNull(schemaTableRow);

            var columnName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.ColumnName);
            var columnOrdinal = (int) schemaTableRow[SchemaTableColumn.ColumnOrdinal];
            var columnSize = (int) schemaTableRow[SchemaTableColumn.ColumnSize];

            var columns = schemaTableRow.Table.Columns;

            short? numericPrecision = null;
            var column = columns[SchemaTableColumn.NumericPrecision];
            if (column != null)
                numericPrecision = schemaTableRow.IsNull(column)
                    ? (short?) null
                    : Convert.ToInt16(schemaTableRow[column]);

            short? numericScale = null;
            column = columns[SchemaTableColumn.NumericScale];
            if (column != null)
                numericScale = schemaTableRow.IsNull(column)
                    ? (short?) null
                    : Convert.ToInt16(schemaTableRow[column]);

            var isUnique = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsUnique);
            var isKey = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsKey);
            //BaseServerName
            //BaseCatalogName
            var baseColumnName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseColumnName);
            var baseSchemaName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseSchemaName);
            var baseTableName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseTableName);
            var dataType = (Type) schemaTableRow[SchemaTableColumn.DataType];
            var allowDbNull = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.AllowDBNull);
            var providerType = schemaTableRow.GetValueField<int>(SchemaTableColumn.ProviderType);

            bool? isAliased = null;
            column = columns[SchemaTableColumn.IsAliased];
            if (column != null)
                isAliased = schemaTableRow.GetNullableValueField<bool>(column);

            bool? isExpression = null;
            column = columns[SchemaTableColumn.IsExpression];
            if (column != null)
                isExpression = schemaTableRow.GetNullableValueField<bool>(column);

            bool? isIdentity = null;
            column = columns["IsIdentity"];
            if (column != null)
                isIdentity = schemaTableRow.GetNullableValueField<bool>(column);

            //IsAutoIncrement
            //IsRowVersion
            //IsHidden
            var isLong = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsLong);
            //IsReadOnly
            //ProviderSpecificDataType
            //DataTypeName XmlSchemaCollectionDatabase XmlSchemaCollectionOwningSchema XmlSchemaCollectionName UdtAssemblyQualifiedName
            int nonVersionedProviderType = 0;
            column = columns[SchemaTableColumn.NonVersionedProviderType];
            if (column != null)
                nonVersionedProviderType = (int) schemaTableRow[column];
            //IsColumnSet

            return new FoundationDbColumn(allowDbNull, baseColumnName, baseSchemaName, baseTableName, columnName, columnOrdinal, columnSize,
                dataType, isAliased, isExpression, isKey, isIdentity, isLong, isUnique, nonVersionedProviderType, numericPrecision, numericScale, providerType);
        }
    }
}