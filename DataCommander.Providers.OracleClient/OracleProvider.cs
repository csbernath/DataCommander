namespace DataCommander.Providers.OracleClient
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.OracleClient;
    using System.Text;
    using System.Xml;
    using DataCommander.Foundation.Data;
    using DataCommander.Providers.OracleBase;

    public sealed class OracleProvider : IProvider
    {
        //void IProvider.DeriveParameters( IDbCommand command )
        //{
        //    OracleCommand command2 = (OracleCommand) command;
        //    OracleCommandBuilder.DeriveParameters( command2 );

        //    //      foreach (OracleParameter parameter in command2.Parameters)
        //    //      {
        //    //        if (parameter.Direction == ParameterDirection.Output)
        //    //        {
        //    //          if (parameter.OracleType == OracleType.VarChar)
        //    //          {
        //    //            parameter.Size = 255;
        //    //          }
        //    //        }
        //    //      }
        //}

        //public DataParameter GetDataParameter( IDataParameter parameter )
        //{
        //    OracleParameter oracleParameter = (OracleParameter) parameter;
        //    return new DataParameterImp( oracleParameter );
        //}

        //DataTable IProvider.GetParameterTable( IDataParameterCollection parameters )
        //{
        //    DataTable dataTable = new DataTable();
        //    dataTable.Columns.Add( "Direction" );
        //    dataTable.Columns.Add( "ParameterName" );
        //    dataTable.Columns.Add( "DbType" );
        //    dataTable.Columns.Add( "OracleType" );
        //    dataTable.Columns.Add( "Size" );
        //    dataTable.Columns.Add( "Precision" );
        //    dataTable.Columns.Add( "Scale" );
        //    dataTable.Columns.Add( "Value" );

        //    foreach (OracleParameter p in parameters)
        //    {
        //        DataRow row = dataTable.NewRow();

        //        row[ 0 ] = p.Direction.ToString();
        //        row[ 1 ] = p.ParameterName;
        //        row[ 2 ] = p.DbType.ToString();
        //        row[ 3 ] = p.OracleType.ToString();
        //        row[ 4 ] = p.Size;
        //        row[ 5 ] = p.Precision;
        //        row[ 6 ] = p.Scale;
        //        row[ 7 ] = p.Value;

        //        dataTable.Rows.Add( row );
        //    }

        //    return dataTable;
        //}

        //public string ToName( OracleType oracleType )
        //{
        //    string name;

        //    switch (oracleType)
        //    {
        //        case OracleType.Number:
        //            name = "NUMBER";
        //            break;

        //        case OracleType.DateTime:
        //            name = "DATE";
        //            break;

        //        case OracleType.Char:
        //            name = "CHAR";
        //            break;

        //        case OracleType.VarChar:
        //            name = "VARCHAR2";
        //            break;

        //        case OracleType.LongVarChar:
        //            name = "LONG";
        //            break;

        //        case OracleType.Clob:
        //            name = "CLOB";
        //            break;

        //        case OracleType.RowId:
        //            name = "ROWID";
        //            break;

        //        default:
        //            name = oracleType.ToString( "G" );
        //            break;
        //    }

        //    return name;
        //}

        //public DataTable GetSchemaTable( IDataReader dataReader )
        //{
        //    //OracleDataReader dataReader2 = (OracleDataReader) dataReader;
        //    //DataTable dataTable = dataReader2.GetSchemaTable();
        //    //DataColumn providerType = dataTable.Columns[ "ProviderType" ];
        //    //DataColumn name = dataTable.Columns.Add( "SqlUtilName", typeof( string ) );
        //    ////DataColumn type = dataTable.Columns.Add( "SqlUtilType", typeof( ColumnType ) );

        //    //foreach (DataRow dataRow in dataTable.Rows)
        //    //{
        //    //    OracleType oracleType = (OracleType) dataRow[ providerType ];
        //    //    dataRow[ name ] = ToName( oracleType );
        //    //    ColumnType columnType;

        //    //    switch (oracleType)
        //    //    {
        //    //        case OracleType.Number:
        //    //            columnType = ColumnType.Numeric;
        //    //            break;

        //    //        case OracleType.DateTime:
        //    //        case OracleType.LongVarChar:
        //    //        case OracleType.Clob:
        //    //        case OracleType.RowId:
        //    //            columnType = ColumnType.Default;
        //    //            break;

        //    //        default:
        //    //            columnType = ColumnType.ColumnSize;
        //    //            break;
        //    //    }

        //    //    dataRow[ type ] = columnType;
        //    //}

        //    //return dataTable;

        //    return null;
        //}

        //public XmlReader ExecuteXmlReader( IDbCommand command )
        //{
        //    return null;
        //}

        //Type IProvider.ToDataType( int providerType, int columnSize )
        //{
        //    return null;
        //}

        //IDataReaderHelper IProvider.CreateDataReaderHelper( IDataReader dataReader )
        //{
        //    return null;
        //}

        ////    public void GetValues(IDataReader dataReader,object[] values)
        ////    {
        ////      OracleDataReader oracleDataReader = (OracleDataReader)dataReader;
        ////      oracleDataReader.GetOracleValues(values);
        ////
        ////      for (int i=0;i<values.Length;i++)
        ////      {
        ////        object value = values[i];
        ////        INullable nullable = (INullable)value;
        ////        
        ////        if (nullable.IsNull)
        ////        {
        ////          values[i] = DBNull.Value;
        ////        }
        ////        else
        ////        {
        ////          Type type = value.GetType();
        ////
        ////          if (type == typeof(OracleDateTime))
        ////          {
        ////            OracleDateTime oracleDateTime = (OracleDateTime)value;
        ////            StringBuilder sb = new StringBuilder();
        ////
        ////            int year = oracleDateTime.Year;
        ////
        ////            if (year >= 0)
        ////              sb.Append(year.ToString().PadLeft(4,'0'));
        ////
        ////            sb.Append('-');
        ////            sb.Append(oracleDateTime.Month.ToString().PadLeft(2,'0'));
        ////            sb.Append('-');
        ////            sb.Append(oracleDateTime.Day.ToString().PadLeft(2,'0'));
        ////            sb.Append(' ');
        ////            sb.Append(oracleDateTime.Hour.ToString().PadLeft(2,'0'));
        ////            sb.Append(':');
        ////            sb.Append(oracleDateTime.Minute.ToString().PadLeft(2,'0'));
        ////            sb.Append(':');
        ////            sb.Append(oracleDateTime.Second.ToString().PadLeft(2,'0'));
        ////
        ////            values[i] = sb.ToString();
        ////          }
        ////          else if (type == typeof(OracleLob))
        ////          {
        ////            OracleLob oracleLob = (OracleLob)value;
        ////            values[i] = oracleLob.Value.ToString();
        ////          }
        ////        }
        ////      }
        ////    }

        //public DbDataAdapter CreateDataAdapter( string selectCommandText, IDbConnection connection )
        //{
        //    return null;
        //}

        //string[] IProvider.GetIntellisense( ConnectionBase connection, string text, int position, out bool fromCache )
        //{
        //    //      string commandText;
        //    //
        //    //      switch (word)
        //    //      {
        //    //        case "from":
        //    //          commandText = "select * from (select table_name from all_tables where owner = '{0}' union select view_name from all_views where owner = '{0}') order by 1";
        //    //          commandText = string.Format(commandText,objectBrowser.SchemasNode.SelectedSchema);
        //    //          break;
        //    //
        //    //        case "table":
        //    //        case "update":
        //    //          commandText = string.Format("select table_name from all_tables where owner='{0}'",connection.DataSource);
        //    //          break;
        //    //
        //    //        default:
        //    //          commandText = null;
        //    //          break;
        //    //      }
        //    //
        //    //      string[] items = null;
        //    //
        //    //      if (commandText != null)
        //    //      {
        //    //        string key = objectBrowser.SchemasNode.SelectedSchema + '.' + word;
        //    //
        //    //        if (completionCache.ContainsKey(key))
        //    //        {
        //    //          items = (string[])completionCache[key];
        //    //        }
        //    //        else
        //    //        {
        //    //          ArrayList list = new ArrayList();
        //    //          IDataReader dataReader = DataHelper.ExecuteReader(commandText,connection.Wrapped);
        //    //
        //    //          while (dataReader.Read())
        //    //            list.Add(dataReader.GetString(0));
        //    //
        //    //          items = new string[list.Count];
        //    //          list.CopyTo(items);
        //    //          completionCache[key] = items;
        //    //        }
        //    //      }
        //    //
        //    //      return  items;
        //    return null;
        //}

        //public void ClearCompletionCache()
        //{
        //    completionCache.Clear();
        //}

        //Hashtable completionCache = new Hashtable();
        ////ObjectExplorer objectBrowser = new ObjectExplorer();

        //#region IProvider Members

        //#endregion

        private static string connectionString;
        private static string[] keyWords;

        #region IProvider Members

        string IProvider.Name
        {
            get
            {
                return "System.Data.OracleClient";
            }
        }

        DbProviderFactory IProvider.DbProviderFactory
        {
            get
            {
#pragma warning disable 618
                return OracleClientFactory.Instance;
#pragma warning restore 618
            }
        }

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            OracleProvider.connectionString = connectionString;
            return new Connection(connectionString);
        }

        string[] IProvider.KeyWords
        {
            get
            {
                if (keyWords == null && OracleProvider.connectionString != null)
                {
                    string connectionString = "Provider=MSDAORA.1;" + OracleProvider.connectionString;
                    keyWords = ProviderFactory.GetKeyWords(connectionString);
                }

                return keyWords;
            }
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return true;
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
            return dataReader.GetSchemaTable();
        }

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            OracleType oracleType = (OracleType) dataColumnSchema[SchemaTableColumn.ProviderType];
            Type type = (Type) dataColumnSchema[SchemaTableColumn.DataType];
            return type;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            OracleDataReader oracleDataReader = (OracleDataReader) dataReader;
            return new OracleDataReaderHelper(oracleDataReader);
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

        void IProvider.ClearCompletionCache()
        {
        }

        string IProvider.GetExceptionMessage(Exception e)
        {
            return e.ToString();
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            return null;
        }

        private static object ConvertDecimalField(object value)
        {
            object convertedValue;

            if (value == DBNull.Value)
            {
                convertedValue = DBNull.Value;
            }
            else
            {
                DecimalField decimalField = (DecimalField) value;
                string stringValue = decimalField.StringValue;
                OracleNumber oracleNumber;

                if (stringValue != null)
                {
                    oracleNumber = OracleNumber.Parse(stringValue);
                }
                else
                {
                    oracleNumber = new OracleNumber(decimalField.DecimalValue);
                }

                convertedValue = oracleNumber;
            }

            return convertedValue;
        }

        void IProvider.CreateInsertCommand(
            DataTable sourceSchemaTable,
            string[] sourceDataTypeNames,
            IDbConnection destinationconnection,
            string destinationTableName,
            out IDbCommand insertCommand,
            out Converter<object, object>[] converters)
        {
            DataTable schemaTable;
            string[] dataTypeNames;
            int count;

            using (IDbCommand command = destinationconnection.CreateCommand())
            {
                command.CommandText = $"select * from {destinationTableName}";

                //OracleDataAdapter adapter = new OracleDataAdapter( (OracleCommand)command );
                //adapter.FillSchema(new DataSet(), SchemaType.Source);
                //OracleCommandBuilder b = new OracleCommandBuilder( adapter );
                //object o = b.GetInsertCommand();

                using (IDataReader dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly))
                {
                    schemaTable = dataReader.GetSchemaTable();
                    count = dataReader.FieldCount;
                    dataTypeNames = new string[count];

                    for (int i = 0; i < count; i++)
                    {
                        dataTypeNames[i] = dataReader.GetDataTypeName(i);
                    }
                }
            }

            StringBuilder insertInto = new StringBuilder();
            insertInto.AppendFormat("insert into {0}(", destinationTableName);
            StringBuilder values = new StringBuilder();
            values.Append("values(");
            DataRowCollection schemaRows = schemaTable.Rows;
            count = schemaRows.Count;
            converters = new Converter<object, object>[count];
            insertCommand = destinationconnection.CreateCommand();

            for (int i = 0; i < count; i++)
            {
                if (i > 0)
                {
                    insertInto.Append(',');
                    values.Append(',');
                }

                var schemaRow = new DataColumnSchema(schemaRows[i]);
                insertInto.Append(schemaRow.ColumnName);
                values.AppendFormat(":p{0}", i + 1);

                int columnSize = schemaRow.ColumnSize;
                OracleType oracleType = (OracleType) schemaRow.ProviderType;
                OracleParameter parameter = new OracleParameter($"p{i + 1}", oracleType);
                insertCommand.Parameters.Add(parameter);
                Converter<object, object> converter;

                switch (oracleType)
                {
                    case OracleType.Number:
                        converter = ConvertDecimalField;
                        break;

                    default:
                        converter = null;
                        break;
                }

                converters[i] = converter;
            }

            insertInto.Append(") ");
            values.Append(')');
            insertInto.Append(values);
            insertCommand.CommandText = insertInto.ToString();
        }

        bool IProvider.CanConvertCommandToString
        {
            get
            {
                return false;
            }
        }

        List<InfoMessage> IProvider.ToInfoMessages(Exception e)
        {
            throw new NotImplementedException();
        }

        List<Statement> IProvider.GetStatements(string commandText)
        {
            return new List<Statement>
            {
                new Statement
                {
                    LineIndex = 0,
                    CommandText = commandText
                }
            };
        }

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text,
            int position)
        {
            throw new NotImplementedException();
        }

        string IProvider.CommandToString(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}