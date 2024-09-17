using System;
using System.Data;
using System.Data.Common;

namespace Foundation.Data;

public static class FoundationDbColumnFactory
{
    public static FoundationDbColumn Create(DataRow schemaTableRow)
    {
        ArgumentNullException.ThrowIfNull(schemaTableRow);

        string columnName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.ColumnName);
        int columnOrdinal = (int)schemaTableRow[SchemaTableColumn.ColumnOrdinal];
        int columnSize = (int)schemaTableRow[SchemaTableColumn.ColumnSize];

        DataColumnCollection columns = schemaTableRow.Table.Columns;

        short? numericPrecision = null;
        DataColumn column = columns[SchemaTableColumn.NumericPrecision];
        if (column != null)
            numericPrecision = schemaTableRow.IsNull(column)
                ? (short?)null
                : Convert.ToInt16(schemaTableRow[column]);

        short? numericScale = null;
        column = columns[SchemaTableColumn.NumericScale];
        if (column != null)
            numericScale = schemaTableRow.IsNull(column)
                ? (short?)null
                : Convert.ToInt16(schemaTableRow[column]);

        bool? isUnique = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsUnique);
        bool? isKey = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsKey);
        //BaseServerName
        //BaseCatalogName
        string baseColumnName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseColumnName);
        string baseSchemaName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseSchemaName);
        string baseTableName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseTableName);

        object dataTypeObject = schemaTableRow[SchemaTableColumn.DataType];
        Type dataType = dataTypeObject != DBNull.Value
            ? (Type)dataTypeObject
            : null;
        bool? allowDbNull = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.AllowDBNull);
        int providerType = schemaTableRow.GetValueField<int>(SchemaTableColumn.ProviderType);

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
        bool? isLong = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsLong);
        //IsReadOnly
        //ProviderSpecificDataType
        //DataTypeName XmlSchemaCollectionDatabase XmlSchemaCollectionOwningSchema XmlSchemaCollectionName UdtAssemblyQualifiedName
        int nonVersionedProviderType = 0;
        column = columns[SchemaTableColumn.NonVersionedProviderType];
        if (column != null)
            nonVersionedProviderType = (int)schemaTableRow[column];
        //IsColumnSet

        return new FoundationDbColumn(allowDbNull, baseColumnName, baseSchemaName, baseTableName, columnName, columnOrdinal, columnSize,
            dataType, isAliased, isExpression, isKey, isIdentity, isLong, isUnique, nonVersionedProviderType, numericPrecision, numericScale, providerType);
    }
}