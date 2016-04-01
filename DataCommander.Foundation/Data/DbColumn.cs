namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentNullException>(schemaTableRow != null);

            this.AllowDBNull = schemaTableRow.Field<bool?>(SchemaTableColumn.AllowDBNull);
            this.BaseColumnName = schemaTableRow.Field<string>(SchemaTableColumn.BaseColumnName);
            this.BaseSchemaName = schemaTableRow.Field<string>(SchemaTableColumn.BaseSchemaName);
            this.BaseTableName = schemaTableRow.Field<string>(SchemaTableColumn.BaseTableName);
            this.ColumnName = schemaTableRow.Field<string>(SchemaTableColumn.ColumnName);
            this.ColumnOrdinal = (int)schemaTableRow[SchemaTableColumn.ColumnOrdinal];
            this.ColumnSize = (int)schemaTableRow[SchemaTableColumn.ColumnSize];
            this.DataType = (Type)schemaTableRow[SchemaTableColumn.DataType];
            this.IsAliased = schemaTableRow.Field<bool?>(SchemaTableColumn.IsAliased);
            this.IsExpression = schemaTableRow.Field<bool?>(SchemaTableColumn.IsExpression);
            this.IsKey = schemaTableRow.Field<bool?>(SchemaTableColumn.IsKey);

            var columns = schemaTableRow.Table.Columns;
            int columnIndex = columns.IndexOf("IsIdentity");
            if (columnIndex >= 0)
            {
                this.IsIdentity = schemaTableRow.Field<bool?>(columnIndex);
            }

            this.IsLong = schemaTableRow.Field<bool?>(SchemaTableColumn.IsLong);
            this.IsUnique = schemaTableRow.Field<bool?>(SchemaTableColumn.IsUnique);

            columnIndex = columns.IndexOf(SchemaTableColumn.NonVersionedProviderType);
            if (columnIndex >= 0)
            {
                this.NonVersionedProviderType = (int)schemaTableRow[columnIndex];
            }

            columnIndex = columns.IndexOf(SchemaTableColumn.NumericPrecision);
            if (columnIndex >= 0)
            {
                this.NumericPrecision = schemaTableRow.Field<short?>(columnIndex);
            }

            this.NumericScale = schemaTableRow.Field<short?>(SchemaTableColumn.NumericScale);
            this.ProviderType = schemaTableRow.Field<int>(SchemaTableColumn.ProviderType);
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