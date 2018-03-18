using System;
using System.Data;
using System.Data.Common;
using Foundation.Diagnostics.Assertions;

namespace Foundation.Data
{
    public sealed class FoundationDbColumn
    {
        public FoundationDbColumn(DataRow schemaTableRow)
        {
            Assert.IsNotNull(schemaTableRow);

            ColumnName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.ColumnName);
            ColumnOrdinal = (int) schemaTableRow[SchemaTableColumn.ColumnOrdinal];
            ColumnSize = (int) schemaTableRow[SchemaTableColumn.ColumnSize];

            var columns = schemaTableRow.Table.Columns;
            var column = columns[SchemaTableColumn.NumericPrecision];
            if (column != null)
            {
                NumericPrecision = schemaTableRow.IsNull(column)
                    ? (short?) null
                    : Convert.ToInt16(schemaTableRow[column]);
            }

            column = columns[SchemaTableColumn.NumericScale];
            if (column != null)
            {
                NumericScale = schemaTableRow.IsNull(column)
                    ? (short?) null
                    : Convert.ToInt16(schemaTableRow[column]);
            }

            IsUnique = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsUnique);
            IsKey = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsKey);
            //BaseServerName
            //BaseCatalogName
            BaseColumnName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseColumnName);
            BaseSchemaName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseSchemaName);
            BaseTableName = schemaTableRow.GetReferenceField<string>(SchemaTableColumn.BaseTableName);
            DataType = (Type) schemaTableRow[SchemaTableColumn.DataType];
            AllowDbNull = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.AllowDBNull);
            ProviderType = schemaTableRow.GetValueField<int>(SchemaTableColumn.ProviderType);

            column = columns[SchemaTableColumn.IsAliased];
            if (column != null)
                IsAliased = schemaTableRow.GetNullableValueField<bool>(column);

            column = columns[SchemaTableColumn.IsExpression];
            if (column != null)
                IsExpression = schemaTableRow.GetNullableValueField<bool>(column);

            column = columns["IsIdentity"];
            if (column != null)
                IsIdentity = schemaTableRow.GetNullableValueField<bool>(column);

            //IsAutoIncrement
            //IsRowVersion
            //IsHidden
            IsLong = schemaTableRow.GetNullableValueField<bool>(SchemaTableColumn.IsLong);
            //IsReadOnly
            //ProviderSpecificDataType
            //DataTypeName XmlSchemaCollectionDatabase XmlSchemaCollectionOwningSchema XmlSchemaCollectionName UdtAssemblyQualifiedName
            column = columns[SchemaTableColumn.NonVersionedProviderType];
            if (column != null)
                NonVersionedProviderType = (int) schemaTableRow[column];
            //IsColumnSet
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? AllowDbNull { get; }

        /// <summary>
        /// 
        /// </summary>
        public string BaseColumnName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string BaseSchemaName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string BaseTableName { get; }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnOrdinal { get; }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnSize { get; }

        /// <summary>
        /// 
        /// </summary>
        public Type DataType { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsAliased { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsExpression { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsKey { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsIdentity { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsLong { get; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsUnique { get; }

        /// <summary>
        /// 
        /// </summary>
        public int NonVersionedProviderType { get; }

        /// <summary>
        /// 
        /// </summary>
        public short? NumericPrecision { get; }

        /// <summary>
        /// 
        /// </summary>
        public short? NumericScale { get; }

        /// <summary>
        /// 
        /// </summary>
        public int ProviderType { get; }
    }
}