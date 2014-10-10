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
            Contract.Requires(schemaTableRow != null);

            this.schemaTableRow = schemaTableRow;
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean? AllowDBNull
        {
            get
            {
                return this.schemaTableRow.Field<Boolean?>(SchemaTableColumn.AllowDBNull);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String BaseColumnName
        {
            get
            {
                return this.schemaTableRow.Field<String>(SchemaTableColumn.BaseColumnName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String BaseSchemaName
        {
            get
            {
                return this.schemaTableRow.Field<String>(SchemaTableColumn.BaseSchemaName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String BaseTableName
        {
            get
            {
                return this.schemaTableRow.Field<String>(SchemaTableColumn.BaseTableName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String ColumnName
        {
            get
            {
                return this.schemaTableRow.Field<String>(SchemaTableColumn.ColumnName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ColumnOrdinal
        {
            get
            {
                return (Int32) this.schemaTableRow[SchemaTableColumn.ColumnOrdinal];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ColumnSize
        {
            get
            {
                return (Int32) this.schemaTableRow[SchemaTableColumn.ColumnSize];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type DataType
        {
            get
            {
                return (Type) this.schemaTableRow[SchemaTableColumn.DataType];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean? IsAliased
        {
            get
            {
                return this.schemaTableRow.Field<Boolean?>(SchemaTableColumn.IsAliased);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean? IsExpression
        {
            get
            {
                return this.schemaTableRow.Field<Boolean?>(SchemaTableColumn.IsExpression);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean? IsKey
        {
            get
            {
                return this.schemaTableRow.Field<Boolean?>(SchemaTableColumn.IsKey);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean? IsLong
        {
            get
            {
                return this.schemaTableRow.Field<Boolean?>(SchemaTableColumn.IsLong);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean? IsUnique
        {
            get
            {
                return this.schemaTableRow.Field<Boolean?>(SchemaTableColumn.IsUnique);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Type NonVersionedProviderType
        {
            get
            {
                return (Type) this.schemaTableRow[SchemaTableColumn.NonVersionedProviderType];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int16? NumericPrecision
        {
            get
            {
                return this.schemaTableRow.Field<Int16?>(SchemaTableColumn.NumericPrecision);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int16? NumericScale
        {
            get
            {
                return this.schemaTableRow.Field<Int16?>(SchemaTableColumn.NumericScale);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 ProviderType
        {
            get
            {
                return this.schemaTableRow.Field<Int32>(SchemaTableColumn.ProviderType);
            }
        }
    }
}