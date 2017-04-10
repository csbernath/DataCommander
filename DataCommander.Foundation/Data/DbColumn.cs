namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DbColumn
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaTableRow"></param>
        public DbColumn(DataRow schemaTableRow)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(schemaTableRow != null);
#endif

            this.ColumnName = schemaTableRow.Field<string>(SchemaTableColumn.ColumnName);
            this.ColumnOrdinal = (int)schemaTableRow[SchemaTableColumn.ColumnOrdinal];
            this.ColumnSize = (int)schemaTableRow[SchemaTableColumn.ColumnSize];

            var columns = schemaTableRow.Table.Columns;
            var column = columns[SchemaTableColumn.NumericPrecision];
            if (column != null)
            {
                this.NumericPrecision = schemaTableRow.IsNull(column)
                    ? (short?)null
                    : Convert.ToInt16(schemaTableRow[column]);
            }

            column = columns[SchemaTableColumn.NumericScale];
            if (column != null)
            {
                this.NumericScale = schemaTableRow.IsNull(column)
                    ? (short?)null
                    : Convert.ToInt16(schemaTableRow[column]);
            }

            this.IsUnique = schemaTableRow.Field<bool?>(SchemaTableColumn.IsUnique);
            this.IsKey = schemaTableRow.Field<bool?>(SchemaTableColumn.IsKey);
            //BaseServerName
            //BaseCatalogName
            this.BaseColumnName = schemaTableRow.Field<string>(SchemaTableColumn.BaseColumnName);
            this.BaseSchemaName = schemaTableRow.Field<string>(SchemaTableColumn.BaseSchemaName);
            this.BaseTableName = schemaTableRow.Field<string>(SchemaTableColumn.BaseTableName);
            this.DataType = (Type)schemaTableRow[SchemaTableColumn.DataType];
            this.AllowDBNull = schemaTableRow.Field<bool?>(SchemaTableColumn.AllowDBNull);
            this.ProviderType = schemaTableRow.Field<int>(SchemaTableColumn.ProviderType);

            column = columns[SchemaTableColumn.IsAliased];
            if (column != null)
            {
                this.IsAliased = schemaTableRow.Field<bool?>(column);
            }

            column = columns[SchemaTableColumn.IsExpression];
            if (column != null)
            {
                this.IsExpression = schemaTableRow.Field<bool?>(column);
            }

            column = columns["IsIdentity"];
            if (column != null)
            {
                this.IsIdentity = schemaTableRow.Field<bool?>(column);
            }

            //IsAutoIncrement
            //IsRowVersion
            //IsHidden
            this.IsLong = schemaTableRow.Field<bool?>(SchemaTableColumn.IsLong);
            //IsReadOnly
            //ProviderSpecificDataType
            //DataTypeName XmlSchemaCollectionDatabase XmlSchemaCollectionOwningSchema XmlSchemaCollectionName UdtAssemblyQualifiedName
            column = columns[SchemaTableColumn.NonVersionedProviderType];
            if (column != null)
            {
                this.NonVersionedProviderType = (int)schemaTableRow[column];
            }
            //IsColumnSet
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? AllowDBNull { get; }

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