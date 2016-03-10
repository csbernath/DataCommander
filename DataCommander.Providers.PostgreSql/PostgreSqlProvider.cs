namespace DataCommander.Providers.PostgreSql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using DataCommander.Providers;
    using Foundation.Data;
    using Foundation.Diagnostics;
    using Npgsql;
    using LogLevel = Foundation.Diagnostics.LogLevel;

    internal sealed class PostgreSqlProvider : IProvider
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();

        string IProvider.Name => "PostgreSql";

        DbProviderFactory IProvider.DbProviderFactory => NpgsqlFactory.Instance;

        string[] IProvider.KeyWords => null;

        bool IProvider.CanConvertCommandToString
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        bool IProvider.IsCommandCancelable
        {
            get
            {
                return true;
            }
        }

        IObjectExplorer IProvider.ObjectExplorer
        {
            get
            {
                return new ObjectExplorer.ObjectExplorer();
            }
        }

        void IProvider.ClearCompletionCache()
        {
            throw new NotImplementedException();
        }

        string IProvider.CommandToString(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        ConnectionBase IProvider.CreateConnection(string connectionString)
        {
            return new Connection(connectionString);
        }

        DbDataAdapter IProvider.CreateDataAdapter(string selectCommandText, IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
        {
            return new PostgreSqlDataReaderHelper((NpgsqlDataReader)dataReader);
        }

        void IProvider.CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationconnection,
            string destinationTableName, out IDbCommand insertCommand, out Converter<object, object>[] converters)
        {
            throw new NotImplementedException();
        }

        void IProvider.DeriveParameters(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        System.Xml.XmlReader IProvider.ExecuteXmlReader(IDbCommand command)
        {
            throw new NotImplementedException();
        }

        Type IProvider.GetColumnType(DataColumnSchema dataColumnSchema)
        {
            // TODO
            return typeof (object);
        }

        string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
        {
            throw new NotImplementedException();
        }

        GetCompletionResponse IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position)
        {
            var response = new GetCompletionResponse
            {
                FromCache = false
            };
            List<IObjectName> array = null;
            var sqlStatement = new SqlStatement(text);
            Token[] tokens = sqlStatement.Tokens;
            Token previousToken, currentToken;
            sqlStatement.FindToken(position, out previousToken, out currentToken);

            if (currentToken != null)
            {
                var parts = new IdentifierParser(new StringReader(currentToken.Value)).Parse().ToList();
                var lastPart = parts.Count > 0 ? parts.Last() : null;
                int lastPartLength = lastPart != null ? lastPart.Length : 0;
                response.StartPosition = currentToken.EndPosition - lastPartLength + 1;
                response.Length = lastPartLength;
                string value = currentToken.Value;
                if (value.Length > 0 && value[0] == '@')
                {
                    if (value.IndexOf("@@") == 0)
                    {
                        // array = keyWords.Where(k => k.StartsWith(value)).Select(keyWord => (IObjectName)new NonSqlObjectName(keyWord)).ToList();
                    }
                    else
                    {
                        var list = new SortedList<string, object>();

                        for (int i = 0; i < tokens.Length; i++)
                        {
                            var token = tokens[i];
                            string keyWord = token.Value;

                            if (keyWord != null && keyWord.Length >= 2 && keyWord.IndexOf(value) == 0 && keyWord != value)
                            {
                                if (!list.ContainsKey(token.Value))
                                {
                                    list.Add(token.Value, null);
                                }
                            }
                        }

                        array = list.Keys.Select(keyWord => (IObjectName)new NonSqlObjectName(keyWord)).ToList();
                    }
                }
            }
            else
            {
                response.StartPosition = position;
                response.Length = 0;
            }

            if (array == null)
            {
                var sqlObject = sqlStatement.FindSqlObject(previousToken, currentToken);
                string commandText = null;

                if (sqlObject != null)
                {
                    DatabaseObjectMultipartName name;
                    int i;

                    switch (sqlObject.Type)
                    {
                        case SqlObjectTypes.Database:
                            // TODO commandText = SqlServerObject.GetDatabases();
                            break;

                        case SqlObjectTypes.Table:
                        case SqlObjectTypes.View:
                        case SqlObjectTypes.Function:
                        case SqlObjectTypes.Table | SqlObjectTypes.View:
                        case SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function:
                        {
                            name = new DatabaseObjectMultipartName(connection.Database, sqlObject.Name);
                            List<string> nameParts = sqlObject.Name != null
                                ? new IdentifierParser(new StringReader(sqlObject.Name)).Parse().ToList()
                                : null;
                            int namePartsCount = nameParts != null ? nameParts.Count : 0;
                            var statements = new List<string>();

                            switch (namePartsCount)
                            {
                                case 0:
                                case 1:
                                {
                                    // statements.Add(SqlServerObject.GetDatabases());
                                    statements.Add(SqlServerObject.GetSchemas());

                                    //var objectTypes = sqlObject.Type.ToObjectTypes();
                                    //statements.Add(SqlServerObject.GetObjects(schema: "dbo", objectTypes: objectTypes));
                                }
                                    break;

                                case 2:
                                    if (nameParts[0] != null)
                                    {
                                        // TODO statements.Add(SqlServerObject.GetSchemas(database: nameParts[0]));

                                        var objectTypes = sqlObject.Type.ToTableTypes();
                                        statements.Add(SqlServerObject.GetTables(schema: nameParts[0], tableTypes: objectTypes));
                                    }
                                    break;

                                case 3:
                                {
                                    if (nameParts[0] != null && nameParts[1] != null)
                                    {
                                        var objectTypes = sqlObject.Type.ToObjectTypes();
                                        statements.Add(SqlServerObject.GetObjects(database: nameParts[0], schema: nameParts[1], objectTypes: objectTypes));
                                    }
                                }
                                    break;
                            }

                            commandText = statements.Count > 0 ? string.Join("\r\n", statements) : null;
                        }
                            break;

                        case SqlObjectTypes.Column:
                            name = new DatabaseObjectMultipartName(connection.Database, sqlObject.ParentName);
                            string[] owners;

                            if (name.Schema != null)
                            {
                                owners = new string[] {name.Schema};
                            }
                            else
                            {
                                owners = new string[] {"dbo", "sys"};
                            }
                            var sb = new StringBuilder();
                            for (i = 0; i < owners.Length; i++)
                            {
                                if (i > 0)
                                {
                                    sb.Append(',');
                                }
                                sb.AppendFormat("'{0}'", owners[i]);
                            }
                            string ownersString = sb.ToString();
                            commandText = string.Format(@"declare @schema_id int
select  top 1 @schema_id = s.schema_id
from    [{0}].sys.schemas s
where   s.name  in({1})

if @schema_id is not null
begin
    declare @object_id int
    select  @object_id = o.object_id
    from    [{0}].sys.all_objects o
    where   o.name = '{2}'
            and o.schema_id = @schema_id
            and o.type in('S','U','TF','V')

    if @object_id is not null
    begin
        select  name
        from [{0}].sys.all_columns c
        where c.object_id = @object_id
        order by column_id
    end
end", name.Database, ownersString, name.Name);
                            break;

                        case SqlObjectTypes.Procedure:
                            name = new DatabaseObjectMultipartName(connection.Database, sqlObject.Name);

                            if (name.Schema == null)
                            {
                                name.Schema = "dbo";
                            }

                            commandText = string.Format(@"select
     s.name
    ,o.name
from [{0}].sys.objects o
join [{0}].sys.schemas s
on o.schema_id = s.schema_id
where   o.type in('P','X')
order by 1", name.Database);
                            break;

                        case SqlObjectTypes.Trigger:
                            commandText = "select name from sysobjects where xtype = 'TR' order by name";
                            break;

                        case SqlObjectTypes.Value:
                            string[] items = sqlObject.ParentName.Split('.');
                            i = items.Length - 1;
                            string columnName = items[i];
                            string tableNameOrAlias = null;
                            if (i > 0)
                            {
                                i--;
                                tableNameOrAlias = items[i];
                            }
                            if (tableNameOrAlias != null)
                            {
                                string tableName;
                                bool contains = sqlStatement.Tables.TryGetValue(tableNameOrAlias, out tableName);
                                if (contains)
                                {
                                    string where;
                                    int tokenIndex = previousToken.Index + 1;
                                    if (tokenIndex < tokens.Length)
                                    {
                                        var token = tokens[tokenIndex];
                                        string tokenValue = token.Value;
                                        int indexofAny = tokenValue.IndexOfAny(new char[] {'\r', '\n'});
                                        if (indexofAny >= 0)
                                        {
                                            tokenValue = tokenValue.Substring(0, indexofAny);
                                        }
                                        string like;
                                        if (tokenValue.Length > 0)
                                        {
                                            if (tokenValue.Contains('%'))
                                            {
                                                like = tokenValue;
                                            }
                                            else
                                            {
                                                like = tokenValue + '%';
                                            }
                                        }
                                        else
                                        {
                                            like = "%";
                                        }
                                        @where = $"where {columnName} like N'{like}'";
                                    }
                                    else
                                    {
                                        @where = null;
                                    }
                                    commandText = $"select distinct top 100 {columnName} from {tableName} (readpast) {@where} order by 1";
                                }
                            }
                            break;
                    }
                }

                if (commandText != null)
                {
                    log.Write(LogLevel.Trace, "commandText:\r\n{0}", commandText);
                    var list = new List<IObjectName>();
                    try
                    {
                        if (connection.State != ConnectionState.Open)
                        {
                            connection.OpenAsync(CancellationToken.None).Wait();
                        }

                        var transactionScope = new DbTransactionScope(connection.Connection, transaction);
                        using (var reader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                        {
                            while (true)
                            {
                                reader.Read(dataRecord =>
                                {
                                    int fieldCount = dataRecord.FieldCount;

                                    string schemaName;
                                    string objectName;

                                    if (fieldCount == 1)
                                    {
                                        schemaName = null;
                                        objectName = dataRecord.GetString(0);
                                    }
                                    else
                                    {
                                        schemaName = dataRecord.GetStringOrDefault(0);
                                        objectName = dataRecord.GetString(1);
                                    }

                                    list.Add(new ObjectName(sqlObject, schemaName, objectName));
                                });

                                if (!reader.NextResult())
                                {
                                    break;
                                }
                            }
                        }
                    }
                    catch
                    {
                    }

                    array = list;
                }
            }

            response.Items = array;
            return response;
        }

        DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
        {
            throw new NotImplementedException();
        }

        string IProvider.GetExceptionMessage(Exception exception)
        {
            return exception.ToString();
        }

        DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
        {
            throw new NotImplementedException();
        }

        DataTable IProvider.GetSchemaTable(IDataReader dataReader)
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

        DataSet IProvider.GetTableSchema(IDbConnection connection, string tableName)
        {
            throw new NotImplementedException();
        }

        List<InfoMessage> IProvider.ToInfoMessages(Exception e)
        {
            throw new NotImplementedException();
        }

        IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder()
        {
            return new ConnectionStringBuilder();
        }
    }
}