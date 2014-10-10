namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using System.Text;
    using Binarit.Foundation.Data;

    internal delegate void SetTransaction( IDbTransaction transaction );

    internal sealed class TransferResultWriter : IResultWriter
    {
        private IProvider destinationProvider;
        private ConnectionBase destinationConnection;
        private string tableName;
        private SetTransaction setTransaction;
        private IDbTransaction transaction;
        private IDbCommand insertCommand;
        private IDbDataParameter[] parameters;
        private Converter<object, object>[] converters; 

        public TransferResultWriter(
            IProvider destinationProvider,
            ConnectionBase destinationConnection,
            string tableName,
            SetTransaction setTransaction)
        {
            this.destinationProvider = destinationProvider;
            this.destinationConnection = destinationConnection;
            this.tableName = tableName;
            this.setTransaction = setTransaction;
        }

        #region IResultWriter Members

        void IResultWriter.BeforeExecuteReader( IProvider provider )
        {
        }

        void IResultWriter.AfterExecuteReader()
        {
        }

        void IResultWriter.WriteBegin()
        {
            this.transaction = this.destinationConnection.Wrapped.BeginTransaction();
            this.setTransaction( this.transaction );
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable, string[] dataTypeName)
        {
            DbCommand command = this.destinationProvider.DbProviderFactory.CreateCommand();
            command.Connection = (DbConnection)this.destinationConnection.Wrapped;
            command.CommandType = CommandType.TableDirect;
            command.CommandText = this.tableName;
            DataTable destinationSchemaTable;

            using (DbDataReader dataReader = command.ExecuteReader( CommandBehavior.SchemaOnly ))
            {
                destinationSchemaTable = dataReader.GetSchemaTable();
            }

            DataRowCollection schemaRows = schemaTable.Rows;
            this.converters = new Converter<object, object>[ schemaRows.Count ];

            for (int i = 0; i < schemaRows.Count; i++)
            {
                this.converters[ i ] = this.destinationProvider.GetConverter( schemaRows[ i ], destinationSchemaTable.Rows[i] );
            }

            StringBuilder insertBuilder = new StringBuilder();
            StringBuilder valuesBuilder = new StringBuilder();
            insertBuilder.AppendFormat( "insert into {0}(", this.tableName );
            valuesBuilder.Append( "values(" );
            bool first = true;
            this.insertCommand = this.destinationConnection.CreateCommand();
            this.insertCommand.Transaction = this.transaction;
            IDataParameterCollection parameters = this.insertCommand.Parameters;
            this.parameters = new IDbDataParameter[ schemaRows.Count ];
            this.converters = new Converter<object, object>[ schemaRows.Count ];

            for (int i = 0; i < schemaRows.Count; i++)
            {
                DataRow schemaRow = schemaRows[ i ];
                string columnName = (string) schemaRow[ SchemaTableColumn.ColumnName ];
                DbType dbType = (DbType) schemaRow[ SchemaTableColumn.ProviderType ];

                if (first)
                {
                    first = false;
                }
                else
                {
                    insertBuilder.Append( ',' );
                    valuesBuilder.Append( ',' );
                }

                insertBuilder.Append( columnName );
                valuesBuilder.AppendFormat( ":{0}", i + 1 );
                IDbDataParameter parameter = this.insertCommand.CreateParameter();
                parameter.DbType = dbType;
                parameters.Add( parameter );
                this.parameters[ i ] = parameter;
                this.converters[ i ] = this.destinationProvider.GetConverter( schemaRow, destinationSchemaTable.Rows[ i ] );
            }

            //insertBuilder.Append( ") " );
            //valuesBuilder.Append( ')' );
            //insertBuilder.Append( valuesBuilder );
            //this.insertCommand.CommandText = insertBuilder.ToString();

            for (int i = 0; i < destinationSchemaTable.Rows.Count; i++)
            {
                DataRow destinationSchemaRow = destinationSchemaTable.Rows[ i ];
                short? precision = destinationSchemaRow.GetValueOrDefault<short?>( SchemaTableColumn.NumericPrecision );
                
                if (precision != null)
                {
                    this.parameters[i].Precision = (byte)precision.Value;
                    short? scale = destinationSchemaRow.GetValueOrDefault<short?>(SchemaTableColumn.NumericScale);

                    if (scale != null)
                    {
                        this.parameters[i].Scale = (byte)scale.Value;
                    }
                }
            }
        }

        void IResultWriter.FirstRowReadBegin()
        {
        }

        void IResultWriter.FirstRowReadEnd()
        {
        }

        void IResultWriter.WriteRows( object[][] rows, int rowCount )
        {
            for (int rowIndex = 0; rowIndex < rowCount; rowIndex++)
            {
                object[] row = rows[ rowIndex ];

                for (int columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    object value = row[ columnIndex ];
                    Converter<object, object> converter = this.converters[ columnIndex ];
                    object destinationValue;

                    if (converter != null)
                    {
                        destinationValue = converter( value );
                    }
                    else
                    {
                        destinationValue = value;
                    }

                    this.parameters[ columnIndex ].Value = destinationValue;
                }

                this.insertCommand.ExecuteNonQuery();
            }
        }

        void IResultWriter.WriteTableEnd( int recordsAffected )
        {
        }

        void IResultWriter.WriteParameters( System.Data.IDataParameterCollection parameters )
        {
        }

        void IResultWriter.WriteEnd()
        {
        }

        #endregion
    }
}