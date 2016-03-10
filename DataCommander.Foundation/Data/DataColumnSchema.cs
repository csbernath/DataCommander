namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class DataColumnSchema
    {
        private readonly DataRow schemaTableRow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="schemaTableRow"></param>
        public DataColumnSchema(DataRow schemaTableRow)
        {
            Contract.Requires<ArgumentNullException>(schemaTableRow != null);

            this.schemaTableRow = schemaTableRow;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name] => this.schemaTableRow[name];

        /// <summary>
        /// 
        /// </summary>
        public bool? AllowDBNull => this.schemaTableRow.Field<bool?>(SchemaTableColumn.AllowDBNull);

        /// <summary>
        /// 
        /// </summary>
        public string BaseColumnName => this.schemaTableRow.Field<string>(SchemaTableColumn.BaseColumnName);

        /// <summary>
        /// 
        /// </summary>
        public string BaseSchemaName => this.schemaTableRow.Field<string>(SchemaTableColumn.BaseSchemaName);

        /// <summary>
        /// 
        /// </summary>
        public string BaseTableName => this.schemaTableRow.Field<string>(SchemaTableColumn.BaseTableName);

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName => this.schemaTableRow.Field<string>(SchemaTableColumn.ColumnName);

        /// <summary>
        /// 
        /// </summary>
        public int ColumnOrdinal => (int) this.schemaTableRow[SchemaTableColumn.ColumnOrdinal];

        /// <summary>
        /// 
        /// </summary>
        public int ColumnSize => (int) this.schemaTableRow[SchemaTableColumn.ColumnSize];

        /// <summary>
        /// 
        /// </summary>
        public Type DataType => (Type) this.schemaTableRow[SchemaTableColumn.DataType];

        /// <summary>
        /// 
        /// </summary>
        public bool? IsAliased => this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsAliased);

        /// <summary>
        /// 
        /// </summary>
        public bool? IsExpression => this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsExpression);

        /// <summary>
        /// 
        /// </summary>
        public bool? IsKey => this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsKey);

        /// <summary>
        /// 
        /// </summary>
        public bool? IsIdentity => this.schemaTableRow.Field<bool?>("IsIdentity");

        /// <summary>
        /// 
        /// </summary>
        public bool? IsLong => this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsLong);

        /// <summary>
        /// 
        /// </summary>
        public bool? IsUnique => this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsUnique);

        /// <summary>
        /// 
        /// </summary>
        public Type NonVersionedProviderType => (Type) this.schemaTableRow[SchemaTableColumn.NonVersionedProviderType];

        /// <summary>
        /// 
        /// </summary>
        public Int16? NumericPrecision => this.schemaTableRow.Field<Int16?>(SchemaTableColumn.NumericPrecision);

        /// <summary>
        /// 
        /// </summary>
        public Int16? NumericScale => this.schemaTableRow.Field<Int16?>(SchemaTableColumn.NumericScale);

        /// <summary>
        /// 
        /// </summary>
        public int ProviderType => this.schemaTableRow.Field<int>(SchemaTableColumn.ProviderType);
    }
}