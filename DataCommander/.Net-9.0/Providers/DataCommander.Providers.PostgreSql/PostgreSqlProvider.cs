using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Foundation.Data;
using Foundation.Log;
using Npgsql;

namespace DataCommander.Providers.PostgreSql;

internal sealed class PostgreSqlProvider : IProvider
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();

    string IProvider.Identifier => "PostgreSql";
    DbProviderFactory IProvider.DbProviderFactory => NpgsqlFactory.Instance;
    string[] IProvider.KeyWords => null;
    bool IProvider.CanConvertCommandToString => throw new NotImplementedException();
    bool IProvider.IsCommandCancelable => true;
    public IObjectExplorer CreateObjectExplorer() => new ObjectExplorer.ObjectExplorer();
    void IProvider.ClearCompletionCache() => throw new NotImplementedException();
    string IProvider.CommandToString(IDbCommand command) => throw new NotImplementedException();
    public string? GetConnectionName(IDbConnection connection) => null;
    ConnectionBase IProvider.CreateConnection(ConnectionStringAndCredential connectionStringAndCredential) => new Connection(connectionStringAndCredential);
    public string GetConnectionName(string connectionString) => throw new NotImplementedException();
    IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader) => new PostgreSqlDataReaderHelper((NpgsqlDataReader)dataReader);

    void IProvider.CreateInsertCommand(DataTable sourceSchemaTable, string[] sourceDataTypeNames, IDbConnection destinationconnection,
        string destinationTableName, out IDbCommand insertCommand, out Converter<object, object>[] converters) => throw new NotImplementedException();

    void IProvider.DeriveParameters(IDbCommand command) => throw new NotImplementedException();
    public Type GetColumnType(FoundationDbColumn dataColumnSchema) => throw new NotImplementedException();

    Type IProvider.GetColumnType(FoundationDbColumn dataColumnSchema)
    {
        // TODO
        return typeof(object);
    }

    string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName) => throw new NotImplementedException();

    public async Task<GetCompletionResult> GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position,
        CancellationToken cancellationToken)
    {
        List<IObjectName> array = null;
        var sqlStatement = new SqlParser(text);
        var tokens = sqlStatement.Tokens;
        sqlStatement.FindToken(position, out var previousToken, out var currentToken);

        int startPosition;
        int length;
        if (currentToken != null)
        {
            var parts = new IdentifierParser(new StringReader(currentToken.Value)).Parse().ToList();
            var lastPart = parts.Count > 0 ? parts.Last() : null;
            var lastPartLength = lastPart != null ? lastPart.Length : 0;
            startPosition = currentToken.EndPosition - lastPartLength + 1;
            length = lastPartLength;
            var value = currentToken.Value;
            if (value.Length > 0 && value[0] == '@')
            {
                if (value.StartsWith("@@"))
                {
                    // array = keyWords.Where(k => k.StartsWith(value)).Select(keyWord => (IObjectName)new NonSqlObjectName(keyWord)).ToList();
                }
                else
                {
                    var list = new SortedList<string, object>();

                    for (var i = 0; i < tokens.Count; i++)
                    {
                        var token = tokens[i];
                        var keyWord = token.Value;

                        if (keyWord != null && keyWord.Length >= 2 && keyWord.StartsWith(value) && keyWord != value)
                            if (!list.ContainsKey(token.Value))
                                list.Add(token.Value, null);
                    }

                    array = list.Keys.Select(keyWord => (IObjectName)new NonSqlObjectName(keyWord)).ToList();
                }
            }
        }
        else
        {
            startPosition = position;
            length = 0;
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
                        var nameParts = sqlObject.Name != null
                            ? new IdentifierParser(new StringReader(sqlObject.Name)).Parse().ToList()
                            : null;
                        var namePartsCount = nameParts != null ? nameParts.Count : 0;
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
                            owners = [name.Schema];
                        }
                        else
                        {
                            owners = ["dbo", "sys"];
                        }

                        var sb = new StringBuilder();
                        for (i = 0; i < owners.Length; i++)
                        {
                            if (i > 0)
                                sb.Append(',');

                            sb.AppendFormat("'{0}'", owners[i]);
                        }

                        var ownersString = sb.ToString();
                        commandText =
                            $@"select c.column_name
from information_schema.columns c
where
    c.table_schema = '{name.Schema}'
    and c.table_name = '{name.Name}'
order by c.ordinal_position";
                        break;

                    case SqlObjectTypes.Procedure:
                        name = new DatabaseObjectMultipartName(connection.Database, sqlObject.Name);

                        if (name.Schema == null)
                            name.Schema = "dbo";

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
                        var items = sqlObject.ParentName.Split('.');
                        i = items.Length - 1;
                        var columnName = items[i];
                        string tableNameOrAlias = null;
                        if (i > 0)
                        {
                            i--;
                            tableNameOrAlias = items[i];
                        }

                        if (tableNameOrAlias != null)
                        {
                            var contains = sqlStatement.Tables.TryGetValue(tableNameOrAlias, out var tableName);
                            if (contains)
                            {
                                string where;
                                var tokenIndex = previousToken.Index + 1;
                                if (tokenIndex < tokens.Count)
                                {
                                    var token = tokens[tokenIndex];
                                    var tokenValue = token.Value;
                                    var indexofAny = tokenValue.IndexOfAny(['\r', '\n']);
                                    if (indexofAny >= 0)
                                    {
                                        tokenValue = tokenValue[..indexofAny];
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
                Log.Write(LogLevel.Trace, "commandText:\r\n{0}", commandText);
                var list = new List<IObjectName>();
                try
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.OpenAsync(CancellationToken.None).Wait();
                    }

                    var executor = connection.Connection.CreateCommandExecutor();
                    //new DbTransactionScope(connection.Connection, transaction);
                    executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
                    {
                        while (true)
                        {
                            var fieldCount = dataReader.FieldCount;
                            while (dataReader.Read())
                            {
                                string schemaName;
                                string objectName;

                                if (fieldCount == 1)
                                {
                                    schemaName = null;
                                    objectName = dataReader.GetString(0);
                                }
                                else
                                {
                                    schemaName = dataReader.GetStringOrDefault(0);
                                    objectName = dataReader.GetString(1);
                                }

                                list.Add(new ObjectName(sqlObject, schemaName, objectName));
                            }

                            if (!dataReader.NextResult())
                                break;
                        }
                    });
                }
                catch
                {
                }

                array = list;
            }
        }

        return new GetCompletionResult(startPosition, length, array, false);
    }

    DataParameterBase IProvider.GetDataParameter(IDataParameter parameter) => throw new NotImplementedException();
    string IProvider.GetExceptionMessage(Exception exception) => exception.ToString();
    DataTable IProvider.GetParameterTable(IDataParameterCollection parameters) => throw new NotImplementedException();
    DataTable IProvider.GetSchemaTable(IDataReader dataReader) => throw new NotImplementedException();

    List<Statement> IProvider.GetStatements(string commandText)
    {
        return
        [
            new(0, commandText)
        ];
    }

    GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string tableName) => throw new NotImplementedException();

    List<InfoMessage> IProvider.ToInfoMessages(Exception e)
    {
        var message = e.ToString();

        return
        [
            InfoMessageFactory.Create(InfoMessageSeverity.Error, null, message)
        ];
    }

    IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder() => new PostgreSqlConnectionStringBuilder();
}