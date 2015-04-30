namespace DataCommander.Providers.OleDb
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.OleDb;
    using System.Text;
    using System.Xml;
    using DataCommander.Foundation.Data;

    internal sealed class DataParameterImp : DataParameterBase
    {
        public DataParameterImp(OleDbParameter parameter)
            : base(parameter, parameter.Size, parameter.Precision, parameter.Scale)
        {
            this.parameter = parameter;
        }

        protected override void SetSize(int size)
        {
            this.parameter.Size = size;
        }

        private readonly OleDbParameter parameter;
    }

    internal sealed class OleDbProvider : IProvider
    {
        private string connectionString;
        private static string[] keyWords;

        string IProvider.Name
        {
            get
            {
                return ProviderName.OleDb;
            }
        }

        DbProviderFactory IProvider.DbProviderFactory
        {
            get
            {
                return OleDbFactory.Instance;
            }
        }

        public ConnectionBase CreateConnection(string connectionString)
        {
            this.connectionString = connectionString;
            return new Connection(connectionString);
        }

        public string[] KeyWords
        {
            get
            {
                if (keyWords == null)
                    keyWords = ProviderFactory.GetKeyWords(this.connectionString);

                return keyWords;
            }
        }

        bool IProvider.CanConvertCommandToString
        {
            get
            {
                return false;
            }
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return false;
            }
        }

        public void DeriveParameters(IDbCommand command)
        {
            OleDbCommand command2 = (OleDbCommand) command;
            OleDbCommandBuilder.DeriveParameters(command2);
        }

        public DataParameterBase GetDataParameter(IDataParameter parameter)
        {
            OleDbParameter oleDbParameter = (OleDbParameter) parameter;
            return new DataParameterImp(oleDbParameter);
        }

        public DataTable GetParameterTable(IDataParameterCollection parameters)
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("ParameterName");
            dataTable.Columns.Add("DbType");
            dataTable.Columns.Add("OleDbType");
            dataTable.Columns.Add("Size");
            dataTable.Columns.Add("Precision");
            dataTable.Columns.Add("Scale");
            dataTable.Columns.Add("Direction");
            dataTable.Columns.Add("Value");

            foreach (OleDbParameter p in parameters)
            {
                DataRow row = dataTable.NewRow();

                row[0] = p.ParameterName;
                row[1] = p.DbType.ToString("G");
                row[2] = p.OleDbType.ToString();
                row[3] = p.Size;
                row[4] = p.Precision;
                row[5] = p.Scale;
                row[6] = p.Direction.ToString("G");
                row[7] = p.Value;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public DataTable GetSchemaTable(IDataReader dataReader)
        {
            //      DataTable dataTable = dataReader.GetSchemaTable();
            //      DataColumn providerType = dataTable.Columns["ProviderType"];
            //      DataColumn name = dataTable.Columns.Add("SqlUtilName",typeof(string));
            //      DataColumn type = dataTable.Columns.Add("SqlUtilType",typeof(ColumnType));
            //
            //      foreach (DataRow dataRow in dataTable.Rows)
            //      { 
            //        OleDbType oleDbType = (OleDbType)dataRow[providerType];
            //        dataRow[name] = oleDbType.ToString("G");
            //
            //        ColumnType columnType;
            //
            //        switch (oleDbType)
            //        {
            //          case OleDbType.Decimal:
            //          case OleDbType.Numeric:
            //            columnType = ColumnType.Numeric;
            //            break;
            //
            //          case OleDbType.BigInt:
            //          case OleDbType.Boolean:
            //          case OleDbType.Currency:
            //          case OleDbType.Date:
            //          case OleDbType.Double:
            //          case OleDbType.Single:
            //          case OleDbType.Integer:
            //          case OleDbType.SmallInt:
            //          case OleDbType.TinyInt:
            //          case OleDbType.UnsignedBigInt:
            //          case OleDbType.UnsignedInt:
            //          case OleDbType.UnsignedSmallInt:
            //          case OleDbType.UnsignedTinyInt:
            //          case OleDbType.Guid:
            //            columnType = ColumnType.Default;
            //            break;
            //
            //          default:
            //            columnType = ColumnType.ColumnSize;
            //            break;
            //        }
            //
            //        dataRow[type] = columnType;
            //      }
            //
            //      return dataTable;

            DataTable table = new DataTable("SchemaTable");
            DataColumnCollection columns = table.Columns;
            columns.Add(" ", typeof (int));
            columns.Add("  ", typeof (string));
            columns.Add("Name", typeof (string));
            columns.Add("Size", typeof (int));
            columns.Add("DbType", typeof (string));
            columns.Add("DataType", typeof (Type));
            columns.Add("ProviderType", typeof (int));

            DataTable schemaTable = dataReader.GetSchemaTable();

            for (int i = 0; i < schemaTable.Rows.Count; i++)
            {
                DataRow row = schemaTable.Rows[i];
                int columnOrdinal = (int) row["ColumnOrdinal"] + 1;
                bool isKey = Database.GetValueOrDefault<bool>(row["IsKey"]);
                string pk = string.Empty;

                if (isKey)
                {
                    pk = "PKEY";
                }

                int columnSize = (int) row["ColumnSize"];
                OleDbType dbType = (OleDbType) row["ProviderType"];
                bool allowDBNull = (bool) row["AllowDBNull"];

                string dataTypeName = dataReader.GetDataTypeName(i);
                var sb = new StringBuilder();
                sb.Append(dataReader.GetDataTypeName(i));

                switch (dbType)
                {
                    case OleDbType.Char:
                    case OleDbType.VarChar:
                    case OleDbType.WChar:
                    case OleDbType.VarWChar:
                    case OleDbType.Binary:
                    case OleDbType.VarBinary:
                        sb.AppendFormat("({0})", columnSize);
                        break;

                    case OleDbType.Decimal:
                        short precision = (short) row["NumericPrecision"];
                        short scale = (short) row["NumericScale"];

                        if (scale == 0)
                            sb.AppendFormat("({0})", precision);
                        else
                            sb.AppendFormat("({0},{1})", precision, scale);
                        break;

                    default:
                        break;
                }

                if (!allowDBNull)
                    sb.Append(" NOT NULL");

                table.Rows.Add(new object[]
                {
                    columnOrdinal,
                    pk,
                    row[SchemaTableColumn.ColumnName],
                    columnSize,
                    sb.ToString(),
                    row["DataType"],
                    row["ProviderType"]
                });
            }

            return table;
        }

        public XmlReader ExecuteXmlReader(IDbCommand command)
        {
            return null;
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            var dbType = (OleDbType) dataColumnSchema.ProviderType;
            Type type;

            switch (dbType)
            {
                default:
                    type = typeof (object);
                    break;
            }

            return type;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            OleDbDataReader oleDbDataReader = (OleDbDataReader) dataReader;
            return new OleDbDataReaderHelper(oleDbDataReader);
        }

        //    public void GetValues(IDataReader dataReader,object[] values)
        //    {
        //      dataReader.GetValues(values);
        //    }

        public DbDataAdapter CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            return null;
        }

        public IObjectExplorer ObjectBrowser
        {
            get
            {
                return new ObjectExplorer();
            }
        }

        public void ClearCompletionCache()
        {
        }

        List<InfoMessage> IProvider.ToInfoMessages(Exception e)
        {
            throw new NotImplementedException();
        }

        public string GetExceptionMessage(Exception e)
        {
            return e.ToString();
        }

        #region IProvider Members

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            return new Connection(connectionString);
        }

        string[] IProvider.KeyWords
        {
            get
            {
                return null;
            }
        }

        void IProvider.DeriveParameters(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            throw new NotImplementedException();
        }

        XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            throw new NotImplementedException();
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                return new ObjectExplorer();
            }
        }

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text,
            int position)
        {
            throw new NotImplementedException();
        }

        void IProvider.ClearCompletionCache()
        {
        }

        string IProvider.GetExceptionMessage(Exception e)
        {
            //OleDbException oleDbException = e as OleDbException;

            //if (oleDbException != null)
            //{
            //}

            return e.ToString();
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            return null;
        }

        void IProvider.CreateInsertCommand(
            DataTable sourceSchemaTable,
            string[] sourceDataTypeNames,
            IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters)
        {
            insertCommand = null;
            converters = null;
        }

        string IProvider.CommandToString(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        List<string> IProvider.GetStatements(string commandText)
        {
            return new List<string>
            {
                commandText
            };
        }

        #endregion
    }
}