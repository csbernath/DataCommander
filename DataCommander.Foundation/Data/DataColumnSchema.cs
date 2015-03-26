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
        /// <param name="name"></param>
        /// <returns></returns>
        public object this[string name]
        {
            get
            {
                return this.schemaTableRow[name];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? AllowDBNull
        {
            get
            {
                return this.schemaTableRow.Field<bool?>(SchemaTableColumn.AllowDBNull);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BaseColumnName
        {
            get
            {
                return this.schemaTableRow.Field<string>(SchemaTableColumn.BaseColumnName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BaseSchemaName
        {
            get
            {
                return this.schemaTableRow.Field<string>(SchemaTableColumn.BaseSchemaName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string BaseTableName
        {
            get
            {
                return this.schemaTableRow.Field<string>(SchemaTableColumn.BaseTableName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string ColumnName
        {
            get
            {
                return this.schemaTableRow.Field<string>(SchemaTableColumn.ColumnName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnOrdinal
        {
            get
            {
                return (int) this.schemaTableRow[SchemaTableColumn.ColumnOrdinal];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ColumnSize
        {
            get
            {
                return (int) this.schemaTableRow[SchemaTableColumn.ColumnSize];
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
        public bool? IsAliased
        {
            get
            {
                return this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsAliased);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsExpression
        {
            get
            {
                return this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsExpression);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsKey
        {
            get
            {
                return this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsKey);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsLong
        {
            get
            {
                return this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsLong);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsUnique
        {
            get
            {
                return this.schemaTableRow.Field<bool?>(SchemaTableColumn.IsUnique);
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
        public int ProviderType
        {
            get
            {
                return this.schemaTableRow.Field<int>(SchemaTableColumn.ProviderType);
            }
        }
    }
}