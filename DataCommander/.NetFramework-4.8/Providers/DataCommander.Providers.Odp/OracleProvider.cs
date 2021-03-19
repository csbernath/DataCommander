using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Text;
using System.Xml;
using DataCommander.Providers.Connection;
using DataCommander.Providers.Odp.DataFieldReader;
using DataCommander.Providers.Query;
using DataCommander.Providers2;
using DataCommander.Providers2.Connection;
using Foundation.Configuration;
using Foundation.Data;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace DataCommander.Providers.Odp
{
    internal sealed class OracleProvider : IProvider
    {
        private string _connectionString;
        private readonly ObjectExplorer.ObjectExplorer _objectExplorer = new ObjectExplorer.ObjectExplorer();

        string IProvider.Name => "Odp";
        DbProviderFactory IProvider.DbProviderFactory => OracleClientFactory.Instance;

        string[] IProvider.KeyWords
        {
            get
            {
                var keyWords = Settings.CurrentType.Attributes["OracleKeyWords"].GetValue<string[]>();
                return keyWords;
            }
        }

        bool IProvider.CanConvertCommandToString => false;
        bool IProvider.IsCommandCancelable => true;

        public void DeriveParameters(IDbCommand command)
        {
            var oracleCommand = (OracleCommand) command;
            OracleCommandBuilder.DeriveParameters(oracleCommand);
            //OracleCommand oracleCommand = (OracleCommand)command;
            //oracleCommand.BindByName = true;
            //string commandText = oracleCommand.CommandText;
            //string[] items = commandText.Split('.');
            //string owner = items[0];
            //string packageName = items[1];
            //string objectName = items[2];

            //CommandBuilder.DeriveParameters(oracleCommand.Connection, owner, packageName, objectName, "1", oracleCommand.Parameters);

            //if (command.Parameters.Count == 0)
            //    CommandBuilder.DeriveParameters(oracleCommand.Connection, owner, packageName, objectName, null, oracleCommand.Parameters);
        }

        public DataParameterBase GetDataParameter(IDataParameter parameter)
        {
            var oracleParameter = (OracleParameter) parameter;
            return new DataParameterImp(oracleParameter);
        }

        public DataTable GetParameterTable(IDataParameterCollection parameters)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("Direction");
            dataTable.Columns.Add("ParameterName");
            dataTable.Columns.Add("DbType");
            dataTable.Columns.Add("OracleDbType");
            dataTable.Columns.Add("Size");
            dataTable.Columns.Add("Precision");
            dataTable.Columns.Add("Scale");
            dataTable.Columns.Add("Value");

            foreach (OracleParameter p in parameters)
            {
                var row = dataTable.NewRow();

                row[0] = p.Direction.ToString();
                row[1] = p.ParameterName;
                row[2] = p.DbType.ToString();
                row[3] = p.OracleDbType.ToString();
                row[4] = p.Size;
                row[5] = p.Precision;
                row[6] = p.Scale;
                row[7] = p.Value;

                dataTable.Rows.Add(row);
            }

            return dataTable;
        }

        public string ToName(OracleDbType oracleType)
        {
            string name;

            switch (oracleType)
            {
                case OracleDbType.Byte:
                case OracleDbType.Int16:
                case OracleDbType.Int32:
                case OracleDbType.Int64:
                case OracleDbType.Decimal:
                case OracleDbType.Single:
                    name = "NUMBER";
                    break;

                case OracleDbType.Date:
                    name = "DATE";
                    break;

                case OracleDbType.Char:
                    name = "CHAR";
                    break;

                case OracleDbType.Varchar2:
                    name = "VARCHAR2";
                    break;

                case OracleDbType.Long:
                    name = "LONG";
                    break;

                case OracleDbType.Clob:
                    name = "CLOB";
                    break;

                case OracleDbType.TimeStamp:
                    name = "TIMESTAMP";
                    break;

                case OracleDbType.Blob:
                    name = "BLOB";
                    break;

                case OracleDbType.XmlType:
                    name = "XMLTYPE";
                    break;


                //        case OracleDbType.row
                //          name = "ROWID";
                //          break;

                default:
                    name = oracleType.ToString("G");
                    break;
            }

            return name;
        }

        public DataTable GetSchemaTable(IDataReader dataReader)
        {
            var table = new DataTable("SchemaTable");
            var columns = table.Columns;
            columns.Add(" ", typeof(int));
            columns.Add("  ", typeof(string));
            columns.Add("Name", typeof(string));
            columns.Add("Size", typeof(int));
            columns.Add("DbType", typeof(string));
            columns.Add("DataType", typeof(Type));
            columns.Add("OracleDbType", typeof(OracleDbType));

            var schemaTable = dataReader.GetSchemaTable();

            for (var i = 0; i < schemaTable.Rows.Count; i++)
            {
                var row = schemaTable.Rows[i];
                var columnOrdinal = (int) row["ColumnOrdinal"] + 1;
                var isKey = row.GetValueField<bool>("IsKey");
                var isRowId = (bool) row["IsRowID"];
                var pk = string.Empty;

                if (isKey)
                    pk = "PKEY";

                if (isRowId)
                {
                    if (pk.Length > 0)
                        pk += ',';

                    pk += "ROWID";
                }

                var dbType = (OracleDbType) row["ProviderType"];
                var columnSize = (int) row["ColumnSize"];
                object columnSizeObject;

                switch (dbType)
                {
                    case OracleDbType.Clob:
                    case OracleDbType.Long:
                        columnSizeObject = DBNull.Value;
                        break;

                    default:
                        columnSizeObject = columnSize;
                        break;
                }

                var allowDbNull = (bool) row["AllowDBNull"];

                var dataTypeName = dataReader.GetDataTypeName(i);
                var sb = new StringBuilder();
                sb.Append(ToName(dbType));

                switch (dbType)
                {
                    case OracleDbType.Char:
                    case OracleDbType.Varchar2:
                        sb.AppendFormat("({0})", columnSize);
                        break;

                    case OracleDbType.Byte:
                    case OracleDbType.Int16:
                    case OracleDbType.Int32:
                    case OracleDbType.Int64:
                    case OracleDbType.Decimal:
                    case OracleDbType.Single:
                        var precision = (short) row["NumericPrecision"];
                        var scale = (short) row["NumericScale"];

                        if (precision == 38 && scale == 127)
                        {
                        }
                        else if (scale == 0)
                        {
                            sb.AppendFormat("({0})", precision);
                        }
                        else
                        {
                            sb.AppendFormat("({0},{1})", precision, scale);
                        }

                        break;

                    default:
                        break;
                }

                if (!allowDbNull)
                    sb.Append(" NOT NULL");

                table.Rows.Add(columnOrdinal, pk, row[SchemaTableColumn.ColumnName], columnSizeObject, sb.ToString(), row["DataType"], dbType);
            }

            return table;
        }

        GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string tableName) => throw new NotImplementedException();

        Type IProvider.GetColumnType(FoundationDbColumn column)
        {
            var oracleDbType = (OracleDbType)column.ProviderType;
            Type type;

            switch (oracleDbType)
            {
                case OracleDbType.Int16:
                    type = typeof (short);
                    break;

                case OracleDbType.Int32:
                    type = typeof (int);
                    break;

                case OracleDbType.Decimal:
                    type = typeof (object);
                    break;

                case OracleDbType.Date:
                    type = typeof (object);
                    break;

                case OracleDbType.Char:
                case OracleDbType.Varchar2:
                    type = typeof (string);
                    break;

                default:
                    type = typeof (object);
                    break;
            }

            return type;
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            //OracleGlobalization globalization = OracleGlobalization.GetThreadInfo();
            //globalization.NumericCharacters = ". ";
            //OracleGlobalization.SetThreadInfo( globalization );
            return new OracleDataReaderHelper(dataReader);
        }

        public DbDataAdapter CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            var oracleConnection = (OracleConnection) connection;
            var dataAdapter = new OracleDataAdapter(selectCommandText, oracleConnection);
            var commandBuilder = new OracleCommandBuilder(dataAdapter);

            try
            {
                dataAdapter.InsertCommand = commandBuilder.GetInsertCommand();
            }
            catch
            {
            }

            try
            {
                dataAdapter.UpdateCommand = commandBuilder.GetUpdateCommand();
            }
            catch
            {
            }

            try
            {
                dataAdapter.DeleteCommand = commandBuilder.GetDeleteCommand();
            }
            catch
            {
            }

            return dataAdapter;
        }

        public IObjectExplorer CreateObjectExplorer() => _objectExplorer;

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            var response = new GetCompletionResponse();
            string[] items = null;
            var sqlStatement = new SqlParser(text);
            sqlStatement.FindToken(position, out var previousToken, out var currentToken);

            if (currentToken != null)
            {
                response.StartPosition = currentToken.StartPosition;
                response.Length = currentToken.EndPosition - currentToken.StartPosition + 1;
            }
            else
            {
                response.StartPosition = position;
                response.Length = 0;
            }

            var sqlObject = sqlStatement.FindSqlObject(previousToken, currentToken);
            string commandText = null;
            var cs = new OracleConnectionStringBuilder(connection.ConnectionString);
            var userId = cs.UserID;

            if (sqlObject != null)
            {
                string[] parts;
                string owner;

                switch (sqlObject.Type)
                {
                    case SqlObjectTypes.Table:
                        var oracleName = new OracleName(userId, sqlObject.Name);
                        commandText = $@"select	TABLE_NAME
from	SYS.ALL_TABLES
where	OWNER	= '{oracleName.Owner}'	
order by TABLE_NAME";
                        sqlObject.ParentName = oracleName.Owner;
                        break;

                    case SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function:
                        var name = sqlObject.Name;

                        if (name != null)
                        {
                            parts = name.Split('.');

                            if (parts.Length > 1)
                            {
                                owner = parts[0].ToUpper();
                                sqlObject.ParentName = owner;
                                name = parts[1];
                            }
                            else
                            {
                                owner = userId;
                                sqlObject.ParentName = owner;
                            }
                        }
                        else
                        {
                            owner = userId;
                            sqlObject.ParentName = owner;
                        }

                        commandText =
                            $@"select	OBJECT_NAME
from	SYS.ALL_OBJECTS
where	OWNER	= '{owner}'
	and OBJECT_TYPE in('TABLE','VIEW')
order by OBJECT_NAME";
                        sqlObject.Name = null;

                        break;

                    case SqlObjectTypes.Column:
                        var parentName = sqlObject.ParentName;

                        if (parentName != null)
                        {
                            parts = parentName.Split('.');
                            string tableName;

                            if (parts.Length == 2)
                            {
                                owner = parts[0].ToUpper();
                                tableName = parts[1].ToUpper();
                            }
                            else
                            {
                                owner = userId;
                                tableName = sqlObject.ParentName.ToUpper();
                                sqlObject.ParentName = owner + '.' + tableName;
                            }

                            commandText =
                                $@"select	COLUMN_NAME
from	SYS.ALL_TAB_COLUMNS
where	OWNER = '{owner}'
	and TABLE_NAME = '{tableName}'
order by COLUMN_ID";
                        }

                        break;

                    case SqlObjectTypes.Function:
                        oracleName = new OracleName(userId, sqlObject.ParentName);
                        commandText =
                            $@"select	OBJECT_NAME
from	SYS.ALL_OBJECTS
where	OWNER	= '{oracleName.Owner
                                }'
	and OBJECT_TYPE	= 'FUNCTION'
order by OBJECT_NAME";
                        sqlObject.ParentName = oracleName.Owner;
                        break;

                    //case SqlObjectTypes.Procedure:
                    //    oracleName = new OracleName( userId, sqlObject.Name );
                    //    break;

                    default:
                        break;
                }
            }

            if (commandText != null)
            {
                var sb = new StringBuilder();
                sb.Append(_objectExplorer.SchemasNode.Connection.DataSource);
                sb.Append('.');
                sb.Append(sqlObject.Type);
                sb.Append('.');
                var parentName = sqlObject.ParentName;

                if (parentName != null)
                    sb.Append(parentName.ToUpper());

                var name = sqlObject.Name;

                if (!string.IsNullOrEmpty(name))
                {
                    sb.Append('.');
                    sb.Append(name.ToUpper());
                }

                var key = sb.ToString();
                var applicationData = DataCommanderApplication.Instance.ApplicationData;
                var folderName = ConfigurationNodeName.FromType(typeof (OracleProvider)) + ConfigurationNode.Delimiter + "CompletionCache";
                var folder = applicationData.CreateNode(folderName);
                var contains = folder.Attributes.TryGetAttributeValue(key, out items);
                response.FromCache = contains;

                if (!contains)
                {
                    var executor = connection.Connection.CreateCommandExecutor();
                    var table = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
                    var count = table.Rows.Count;
                    items = new string[count];

                    for (var i = 0; i < count; i++)
                    {
                        items[i] = (string) table.Rows[i][0];
                    }

                    folder.Attributes.Add(key, items, null);
                }
            }

            //			SqlStatement sqlStatement = new SqlStatement(text);
            //			SqlObject sqlObject = sqlStatement.FindSqlObject(position);
            //			string commandText = null;
            //
            //			switch (sqlObject.Type)
            //			{
            //				case SqlObjectType.TableOrView:
            //					string owner = connection.Database;
            //					commandText = string.Format("select * from (select table_name from all_tables where owner = '{0}' union select view_name from all_views where owner = '{0}') order by 1",owner);
            //					break;
            //			}

            //      string commandText;
            //
            //      switch (word)
            //      {
            //        case "from":
            //          commandText = "select * from (select table_name from all_tables where owner = '{0}' union select view_name from all_views where owner = '{0}') order by 1";
            //          commandText = string.Format(commandText,objectBrowser.SchemasNode.SelectedSchema);
            //          break;
            //
            //        case "table":
            //        case "update":
            //          commandText = string.Format("select table_name from all_tables where owner='{0}'",connection.DataSource);
            //          break;
            //
            //        default:
            //          commandText = null;
            //          break;
            //      }
            //
            //      string[] items = null;
            //
            //      if (commandText != null)
            //      {
            //        string key = objectBrowser.SchemasNode.Connection.DataSource + "." + 
            //          objectBrowser.SchemasNode.SelectedSchema + '.' + word;
            //
            //        Folder appData = Application.Instance.ApplicationData.CurrentType;
            //
            //        if (appData.SubFolders.Contains("CompletionCache"))
            //        {
            //          appData = (Folder)appData.SubFolders["CompletionCache"];
            //        }
            //        else
            //        {
            //          Folder folder = new Folder(appData,"CompletionCache");
            //        }
            //        
            //        bool containsKey = appData.Properties.ContainsKey(key);
            //
            //        if (containsKey)
            //        {
            //          items = (string[])appData.Properties[key];
            //        }
            //        else
            //        {
            //            Cursor.Current = Cursors.WaitCursor;
            //            ArrayList list = new ArrayList();
            //            IDataReader dataReader = DataHelper.ExecuteReader(commandText,connection.Wrapped);
            //
            //            while (dataReader.Read())
            //              list.Add(dataReader.GetString(0));
            //
            //            items = new string[list.Count];
            //            list.CopyTo(items);
            //
            //            appData.Properties[key] = items;
            //
            //            Cursor.Current = Cursors.Default;
            //        }
            //      }
            //
            //      return items;

            throw new NotImplementedException();

            //response.Items = items;
            //return response;
        }

        public void ClearCompletionCache()
        {
            var folder = DataCommanderApplication.Instance.ApplicationData.CurrentType;
            var folder2 = folder.ChildNodes["CompletionCache"];

            if (folder2 != null)
                folder.RemoveChildNode(folder2);
        }

        public string GetExceptionMessage(Exception e)
        {
            string message;

            var oex = e as OracleException;

            if (oex != null)
            {
                var sb = new StringBuilder();

                foreach (OracleError oe in oex.Errors)
                {
                    sb.AppendFormat("Number: {0}", oe.Number);

                    if (oe.Procedure.Length > 0)
                        sb.AppendFormat(", Procedure: {0}", oe.Procedure);

                    sb.AppendFormat("\r\n{0}\r\n", oe.Message);
                }

                message = sb.ToString();
            }
            else
                message = e.ToString();

            return message;
        }

        #region IProvider Members

        private static object ConvertDecimal(object value)
        {
            object result;

            if (value == DBNull.Value)
            {
                result = DBNull.Value;
            }
            else
            {
                var type = value.GetType();
                var typeCode = Type.GetTypeCode(type);

                if (typeCode == TypeCode.String)
                {
                    var s = (string) value;

                    //OracleGlobalization oracleGlobalization = OracleGlobalization.GetThreadInfo();
                    //oracleGlobalization.NumericCharacters = ". ";
                    //OracleGlobalization.SetThreadInfo( oracleGlobalization );

                    result = new OracleDecimal(s).Value;
                }
                else
                {
                    throw new NotImplementedException();
                }

                //DecimalField decimalField = (DecimalField) value;
                //string stringValue = decimalField.StringValue;

                //if (stringValue != null)
                //{
                //    OracleDecimal oracleDecimal = new OracleDecimal( stringValue );
                //    result = oracleDecimal;
                //}
                //else
                //{
                //    result = decimalField.DecimalValue;
                //}
            }

            return result;
        }

        private static object ConvertObject(object value)
        {
            object result;

            if (value == DBNull.Value)
            {
                result = DBNull.Value;
            }
            else
            {
                result = value.ToString();
            }

            return result;
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

        #endregion

        #region IProvider Members


        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            _connectionString = connectionString;
            return new Connection(connectionString);
        }

        void IProvider.DeriveParameters(IDbCommand command) => throw new NotImplementedException();
        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter) => throw new NotImplementedException();
        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters) => throw new NotImplementedException();

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
        {
            // TODO
            return dataReader.GetSchemaTable();
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        void IProvider.ClearCompletionCache()
        {
            // TODO
        }

        string IProvider.GetExceptionMessage(Exception e)
        {
            // TODO
            return e.ToString();
        }

        List<InfoMessage> IProvider.ToInfoMessages(Exception e) => throw new NotImplementedException();
        string IProvider.CommandToString(IDbCommand command) => throw new NotImplementedException();

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

        IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder() => new ConnectionStringBuilder();
        public Type GetColumnType(FoundationDbColumn dataColumnSchema) => throw new NotImplementedException();

        #endregion
    }
}