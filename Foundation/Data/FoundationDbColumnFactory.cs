using System;
using System.Data;
using System.Data.Common;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Data
{
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