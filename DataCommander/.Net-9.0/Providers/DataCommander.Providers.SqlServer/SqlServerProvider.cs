using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using DataCommander.Providers.SqlServer.FieldReader;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.FieldReaders;
using DataCommander.Api.Query;
using DataCommander.Providers.SqlServer.ObjectExplorer;
using Foundation.Configuration;
using Foundation.Core;
using Foundation.Data;
using Foundation.Data.SqlClient;
using Foundation.Linq;
using Foundation.Log;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer;

internal sealed class SqlServerProvider : IProvider
{
    static SqlServerProvider()
    {
        var configurationNode = Settings.CurrentType;
        ShortStringSize = configurationNode.Attributes["ShortStringSize"].GetValue<int>();
    }

    public static int ShortStringSize { get; }

    internal static List<InfoMessage> ToInfoMessages(SqlErrorCollection sqlErrors, DateTime creationTime)
    {
        ArgumentNullException.ThrowIfNull(sqlErrors);

        List<InfoMessage> messages = new List<InfoMessage>(sqlErrors.Count);

        foreach (SqlError sqlError in sqlErrors)
        {
            var severity = sqlError.Class == 0
                ? InfoMessageSeverity.Information
                : InfoMessageSeverity.Error;

            var header = sqlError.GetHeader();
            var message = sqlError.Message;
            messages.Add(InfoMessageFactory.Create(severity, header, message));
        }

        return messages;
    }

    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    private static string[] _keyWords;

    string IProvider.Identifier => ProviderIdentifier.SqlServer;
    DbProviderFactory IProvider.DbProviderFactory => SqlClientFactory.Instance;

    public string? GetConnectionName(IDbConnection connection)
    {
        var sqlConnection = (SqlConnection)connection;
        return ConnectionNameProvider.GetConnectionName(sqlConnection);
    }

    ConnectionBase IProvider.CreateConnection(ConnectionStringAndCredential connectionStringAndCredential) => new Connection(connectionStringAndCredential);

    string[] IProvider.KeyWords
    {
        get
        {
            if (_keyWords == null)
            {
                var folder = Settings.CurrentType;
                _keyWords = folder.Attributes["TSqlKeyWords"].GetValue<string[]>()!;
            }

            return _keyWords;
        }
    }

    bool IProvider.CanConvertCommandToString => true;

    bool IProvider.IsCommandCancelable => true;

    public IObjectExplorer CreateObjectExplorer() => new ObjectExplorer.ObjectExplorer();

    public void ClearCompletionCache()
    {
    }

    string IProvider.CommandToString(IDbCommand command)
    {
        var sqlCommand = (SqlCommand)command;
        return sqlCommand.ToLogString();
    }

    IDbConnectionStringBuilder IProvider.CreateConnectionStringBuilder() => new SqlServerConnectionStringBuilder();

    IDataReaderHelper IProvider.CreateDataReaderHelper(IDataReader dataReader)
    {
        var sqlDataReaderHelper = new SqlDataReaderHelper(dataReader);
        return sqlDataReaderHelper;
    }

    void IProvider.CreateInsertCommand(
        DataTable sourceSchemaTable,
        string[] sourceDataTypeNames,
        IDbConnection destinationConnection,
        string? destinationTableName,
        out IDbCommand insertCommand,
        out Converter<object, object>[] converters)
    {
        DataTable schemaTable;
        string[] dataTypeNames;
        int count;

        EnumerableRowCollection<string> sourceColumnNames =
            from sourceSchemaRow in sourceSchemaTable.AsEnumerable()
            select FoundationDbColumnFactory.Create(sourceSchemaRow).ColumnName;

        using (var command = destinationConnection.CreateCommand())
        {
            command.CommandText = $"select {string.Join(",", sourceColumnNames)} from {destinationTableName}";
            command.CommandType = CommandType.Text;

            using var dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly);
            schemaTable = dataReader.GetSchemaTable();
            count = dataReader.FieldCount;
            dataTypeNames = new string[count];

            for (var i = 0; i < count; i++) dataTypeNames[i] = dataReader.GetDataTypeName(i);
        }

        var insertInto = new StringBuilder();
        insertInto.AppendFormat("insert into [{0}](", destinationTableName);
        var values = new StringBuilder();
        values.Append("values(");
        var schemaRows = schemaTable.Rows;
        count = schemaRows.Count;
        converters = new Converter<object, object>[count];
        insertCommand = destinationConnection.CreateCommand();

        for (var i = 0; i < count; i++)
        {
            if (i > 0)
            {
                insertInto.Append(',');
                values.Append(',');
            }

            var columnSchema = FoundationDbColumnFactory.Create(schemaRows[i]);
            insertInto.AppendFormat("[{0}]", columnSchema.ColumnName);
            values.AppendFormat("@p{0}", i);

            var columnSize = columnSchema.ColumnSize;
            var providerType = columnSchema.ProviderType;
            var dbType = (DbType)providerType;
            var parameter = new SqlParameter
            {
                ParameterName = $"@p{i}"
            };
            //parameter.DbType = dbType;
            insertCommand.Parameters.Add(parameter);

            switch (dataTypeNames[i].ToLower())
            {
                case SqlDataTypeName.BigInt:
                    parameter.SqlDbType = SqlDbType.BigInt;
                    break;

                case SqlDataTypeName.Bit:
                    parameter.SqlDbType = SqlDbType.Bit;
                    break;

                case SqlDataTypeName.DateTime:
                    parameter.SqlDbType = SqlDbType.DateTime;
                    break;

                case SqlDataTypeName.Float:
                    parameter.SqlDbType = SqlDbType.Float;
                    break;

                case SqlDataTypeName.Int:
                    parameter.SqlDbType = SqlDbType.Int;
                    break;

                case SqlDataTypeName.SmallDateTime:
                    parameter.SqlDbType = SqlDbType.SmallDateTime;
                    break;

                case SqlDataTypeName.SmallInt:
                    parameter.SqlDbType = SqlDbType.SmallInt;
                    break;

                case SqlDataTypeName.TinyInt:
                    parameter.SqlDbType = SqlDbType.TinyInt;
                    break;

                case SqlDataTypeName.VarChar:
                case SqlDataTypeName.NVarChar:
                case SqlDataTypeName.Char:
                case SqlDataTypeName.NChar:
                    parameter.Size = columnSchema.ColumnSize;
                    converters[i] = ConvertToString;
                    break;

                case SqlDataTypeName.NText:
                    parameter.SqlDbType = SqlDbType.NText;
                    converters[i] = ConvertToString;
                    break;

                case SqlDataTypeName.Decimal:
                    parameter.SqlDbType = SqlDbType.Decimal;
                    parameter.Precision = (byte)columnSchema.NumericPrecision.Value;
                    parameter.Scale = (byte)columnSchema.NumericScale.Value;
                    converters[i] = ConvertToDecimal;
                    break;

                case SqlDataTypeName.Money:
                    parameter.SqlDbType = SqlDbType.Money;
                    converters[i] = ConvertToDecimal;
                    break;

                case SqlDataTypeName.Xml:
                    parameter.SqlDbType = SqlDbType.Xml;
                    converters[i] = ConvertToString;
                    break;
            }
        }

        insertInto.Append(") ");
        values.Append(')');
        insertInto.Append(values);
        insertCommand.CommandText = insertInto.ToString();
    }

    void IProvider.DeriveParameters(IDbCommand command)
    {
        var sqlConnection = (SqlConnection)command.Connection;
        var sqlCommand = new SqlCommand(command.CommandText, sqlConnection)
        {
            CommandType = command.CommandType,
            CommandTimeout = command.CommandTimeout
        };

        SqlCommandBuilder.DeriveParameters(sqlCommand);
        command.Parameters.Clear();
        while (sqlCommand.Parameters.Count > 0)
        {
            var parameter = sqlCommand.Parameters[0];
            sqlCommand.Parameters.RemoveAt(0);
            command.Parameters.Add(parameter);
        }
    }

    public static XmlReader ExecuteXmlReader(IDbCommand command)
    {
        var sqlCommand = (SqlCommand)command;
        return sqlCommand.ExecuteXmlReader();
    }

    string IProvider.GetColumnTypeName(IProvider sourceProvider, DataRow sourceSchemaRow, string sourceDataTypeName)
    {
        var schemaRow = FoundationDbColumnFactory.Create(sourceSchemaRow);
        var columnSize = schemaRow.ColumnSize;
        var allowDbNull = schemaRow.AllowDbNull;
        var dataType = schemaRow.DataType;
        var typeCode = Type.GetTypeCode(dataType);
        var typeName = typeCode switch
        {
            TypeCode.Int32 => SqlDataTypeName.Int,
            TypeCode.DateTime => SqlDataTypeName.DateTime,
            TypeCode.Double => SqlDataTypeName.Float,
            TypeCode.String => $"{SqlDataTypeName.NVarChar}({columnSize})",
            _ => $"'{typeCode}'",// TODO
        };
        return typeName;
    }

    Type IProvider.GetColumnType(FoundationDbColumn column)
    {
        var dbType = (SqlDbType)column.ProviderType;
        var columnSize = column.ColumnSize;
        Type type;

        switch (dbType)
        {
            case SqlDbType.BigInt:
                type = typeof(long);
                break;

            case SqlDbType.Bit:
                type = typeof(bool);
                break;

            case SqlDbType.DateTime:
                //type = typeof(DateTimeField); DataTableView does not work in edit mode
                type = typeof(object);
                break;

            case SqlDbType.Int:
                type = typeof(int);
                break;

            case SqlDbType.Char:
            case SqlDbType.NChar:
            case SqlDbType.VarChar:
            case SqlDbType.NVarChar:
            case SqlDbType.Text:
            case SqlDbType.NText:
                if (columnSize <= 8000)
                    type = typeof(string);
                else
                    type = typeof(object);

                break;

            case SqlDbType.SmallInt:
                type = typeof(short);
                break;

            default:
                type = typeof(object);
                break;
        }

        return type;
    }

    async Task<GetCompletionResult> IProvider.GetCompletion(ConnectionBase connection, IDbTransaction transaction, string text, int position,
        CancellationToken cancellationToken)
    {
        var fromCache = false;
        List<IObjectName> array = null;
        var sqlStatement = new SqlParser(text);
        var tokens = sqlStatement.Tokens;
        sqlStatement.FindToken(position, out var previousToken, out var currentToken);
        int startPosition;
        int length;

        if (currentToken != null)
        {
            List<string> parts = new IdentifierParser(new StringReader(currentToken.Value)).Parse().ToList();
            var lastPart = parts.Count > 0
                ? parts.Last()
                : null;
            var lastPartLength = lastPart != null
                ? lastPart.Length
                : 0;
            startPosition = currentToken.EndPosition - lastPartLength + 1;
            length = lastPartLength;
            var value = currentToken.Value;
            if (value.Length > 0 && value[0] == '@')
            {
                if (value.StartsWith("@@"))
                {
                    array = _keyWords.Where(k => k.StartsWith(value)).Select(keyWord => (IObjectName)new NonSqlObjectName(keyWord)).ToList();
                }
                else
                {
                    SortedList<string?, object> list = [];

                    for (var i = 0; i < tokens.Count; i++)
                    {
                        var token = tokens[i];
                        var keyWord = token.Value;

                        if (keyWord != null && keyWord.Length >= 2 && keyWord.StartsWith(value) && keyWord != value && !list.ContainsKey(token.Value))
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
            string? commandText = null;

            if (sqlObject != null)
            {
                DatabaseObjectMultipartName name;
                int i;

                switch (sqlObject.Type)
                {
                    case SqlObjectTypes.Database:
                        commandText = SqlServerObject.GetDatabases();
                        break;

                    case SqlObjectTypes.Table:
                    case SqlObjectTypes.View:
                    case SqlObjectTypes.Function:
                    case SqlObjectTypes.Table | SqlObjectTypes.View:
                    case SqlObjectTypes.Table | SqlObjectTypes.View | SqlObjectTypes.Function:
                    {
                        name = new DatabaseObjectMultipartName(connection.Database, sqlObject.Name);
                            List<string>? nameParts = sqlObject.Name != null
                            ? new IdentifierParser(new StringReader(sqlObject.Name)).Parse().ToList()
                            : null;
                            var namePartsCount = nameParts != null
                            ? nameParts.Count
                            : 0;
                            List<string> statements = [];

                        switch (namePartsCount)
                        {
                            case 0:
                            case 1:
                            {
                                statements.Add(SqlServerObject.GetDatabases());
                                statements.Add(SqlServerObject.GetSchemas());

                                        List<string> objectTypes = sqlObject.Type.ToObjectTypes();
                                statements.Add(SqlServerObject.GetObjects("dbo", objectTypes));
                            }
                                break;

                            case 2:
                                if (nameParts[0] != null)
                                {
                                    statements.Add(SqlServerObject.GetSchemas(nameParts[0]));

                                        List<string> objectTypes = sqlObject.Type.ToObjectTypes();
                                    statements.Add(SqlServerObject.GetObjects(nameParts[0], objectTypes));
                                }

                                break;

                            case 3:
                            {
                                if (nameParts[0] != null && nameParts[1] != null)
                                {
                                            List<string> objectTypes = sqlObject.Type.ToObjectTypes();
                                    statements.Add(SqlServerObject.GetObjects(nameParts[0], nameParts[1], objectTypes));
                                }
                            }
                                break;
                        }

                        commandText = statements.Count > 0
                            ? string.Join("\r\n", statements)
                            : null;
                    }
                        break;

                    case SqlObjectTypes.Column:
                        name = new DatabaseObjectMultipartName(connection.Database, sqlObject.ParentName);
                        string?[] owners;

                        if (name.Schema != null)
                            owners = [name.Schema];
                        else
                            owners = ["dbo", "sys"];

                        var sb = new StringBuilder();
                        for (i = 0; i < owners.Length; i++)
                        {
                            if (i > 0) sb.Append(',');

                            sb.AppendFormat("'{0}'", owners[i]);
                        }

                        var ownersString = sb.ToString();
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
                            name.Schema = "dbo";

                        commandText = SqlServerObject.GetObjectsByDatabase(name.Database, ["P", "X"]);
                        break;

                    case SqlObjectTypes.Trigger:
                        commandText = "select name from sysobjects where xtype = 'TR' order by name";
                        break;

                    case SqlObjectTypes.Value:
                        string[] items = sqlObject.ParentName.Split('.');
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
                                    var tokenValue = token.Value!;
                                    var indexofAny = tokenValue.IndexOfAny(['\r', '\n']);
                                    if (indexofAny >= 0) tokenValue = tokenValue[..indexofAny];

                                    string? like;
                                    if (tokenValue.Length > 0)
                                    {
                                        if (tokenValue.Contains('%'))
                                            like = tokenValue;
                                        else
                                            like = tokenValue + '%';
                                    }
                                    else
                                    {
                                        like = "%";
                                    }

                                    where = $"where {columnName} like N'{like}'";
                                }
                                else
                                    where = null;

                                commandText = $@"select distinct {columnName}
from
(
    select top 1000 {columnName}
    from {tableName} (readpast)
    {where}
) t";
                            }
                        }

                        break;
                }
            }

            if (commandText != null)
            {
                Log.Write(LogLevel.Trace, "commandText:\r\n{0}", commandText);
                List<IObjectName> list = [];
                try
                {
                    if (connection.State != ConnectionState.Open)
                        connection.OpenAsync(CancellationToken.None).Wait();

                    var executor = connection.Connection.CreateCommandAsyncExecutor();
                    await executor.ExecuteReaderAsync(new ExecuteReaderRequest(commandText, null, transaction), async (dataReader, cancellationToken2) =>
                    {
                        while (true)
                        {
                            var fieldCount = dataReader.FieldCount;
                            while (await dataReader.ReadAsync(cancellationToken2))
                            {
                                string? schemaName;
                                string objectName;

                                if (fieldCount == 1)
                                {
                                    schemaName = null;
                                    objectName = dataReader[0].ToString();
                                }
                                else
                                {
                                    schemaName = dataReader.GetStringOrDefault(0);
                                    objectName = dataReader.GetString(1);
                                }

                                list.Add(new ObjectName(schemaName, objectName));
                            }

                            if (!await dataReader.NextResultAsync(cancellationToken2))
                                break;
                        }
                    }, cancellationToken);
                }
                catch
                {
                }

                array = list;
            }
        }

        return new GetCompletionResult(startPosition, length, array, fromCache);
    }

    DataParameterBase IProvider.GetDataParameter(IDataParameter parameter)
    {
        var sqlParameter = (SqlParameter)parameter;
        return new SqlDataParameter(sqlParameter);
    }

    public string GetExceptionMessage(Exception exception)
    {
        var message = exception is SqlException sqlException
            ? sqlException.Errors.ToLogString()
            : exception.ToString();
        return message;
    }

    DataTable IProvider.GetParameterTable(IDataParameterCollection parameters)
    {
        var dataTable = new DataTable();
        dataTable.Columns.Add(" ");
        dataTable.Columns.Add("ParameterName");
        dataTable.Columns.Add("DbType");
        dataTable.Columns.Add("SqlDbType");
        dataTable.Columns.Add("Size", typeof(int));
        dataTable.Columns.Add("Precision", typeof(int));
        dataTable.Columns.Add("Scale", typeof(int));
        dataTable.Columns.Add("Direction");
        dataTable.Columns.Add("Value", typeof(object));
        dataTable.Columns.Add("TypeName", typeof(string));
        var index = 0;

        foreach (SqlParameter p in parameters)
        {
            var row = dataTable.NewRow();

            row[0] = index;
            row[1] = p.ParameterName;
            row[2] = p.DbType.ToString("G");
            row[3] = p.SqlDbType.ToString().ToLower();

            var precision = p.Precision;
            int size;

            if (precision > 0)
            {
                if (precision <= 9)
                    size = 5;
                else if (precision <= 19)
                    size = 9;
                else if (precision <= 28)
                    size = 13;
                else
                    size = 17;
            }
            else
            {
                size = p.Size;
            }

            row[4] = size;
            row[5] = p.Precision;
            row[6] = p.Scale;
            row[7] = p.Direction.ToString("G");

            if (p.Value == null)
                row[8] = DBNull.Value;
            else
                row[8] = p.Value;

            row[9] = p.TypeName;

            dataTable.Rows.Add(row);

            index++;
        }

        return dataTable;
    }

    DataTable IProvider.GetSchemaTable(IDataReader dataReader)
    {
        DataTable table = null;
        var schemaTable = dataReader.GetSchemaTable();

        if (schemaTable != null)
        {
            Log.Trace(CallerInformation.Create(), schemaTable.ToStringTableString());
            Log.Trace(CallerInformation.Create(), "{0}", schemaTable.TableName);

            table = new DataTable("SchemaTable");
            var columns = table.Columns;
            columns.Add(" ", typeof(int));
            columns.Add("  ", typeof(string));
            columns.Add("Name", typeof(string));
            columns.Add("Size", typeof(int));
            columns.Add("DbType", typeof(string));
            columns.Add("DataType", typeof(Type));
            var columnIndex = 0;
            int? columnOrdinalAddition = null;

            foreach (DataRow dataRow in schemaTable.Rows)
            {
                var dataColumnSchema = FoundationDbColumnFactory.Create(dataRow);
                var columnOrdinal = dataColumnSchema.ColumnOrdinal;

                if (columnOrdinalAddition == null)
                {
                    if (columnOrdinal == 0)
                        columnOrdinalAddition = 1;
                    else
                        columnOrdinalAddition = 0;
                }

                var primaryKey = GetPrimaryKey(dataColumnSchema);

                var columnSize = dataColumnSchema.ColumnSize;
                var dbType = (SqlDbType)dataColumnSchema.ProviderType;
                var dataTypeName = dataReader.GetDataTypeName(columnIndex);
                var sb = new StringBuilder();
                sb.Append(dataTypeName);

                switch (dbType)
                {
                    case SqlDbType.Char:
                    case SqlDbType.VarChar:
                    case SqlDbType.NChar:
                    case SqlDbType.NVarChar:
                    case SqlDbType.Binary:
                    case SqlDbType.VarBinary:
                        string columnSizeString;

                        if (columnSize == int.MaxValue)
                            columnSizeString = "max";
                        else
                            columnSizeString = columnSize.ToString();

                        sb.AppendFormat("({0})", columnSizeString);
                        break;

                    case SqlDbType.Decimal:
                        var precision = dataColumnSchema.NumericPrecision.GetValueOrDefault();
                        var scale = dataColumnSchema.NumericScale.GetValueOrDefault();

                        if (scale == 0)
                            sb.AppendFormat("({0})", precision);
                        else
                            sb.AppendFormat("({0},{1})", precision, scale);

                        if (precision <= 9)
                            columnSize = 5;
                        else if (precision <= 19)
                            columnSize = 9;
                        else if (precision <= 28)
                            columnSize = 13;
                        else
                            columnSize = 17;
                        break;
                }

                var allowDbNull = dataColumnSchema.AllowDbNull.GetValueOrDefault();
                if (!allowDbNull) sb.Append(" not null");

                table.Rows.Add(columnOrdinal + columnOrdinalAddition, primaryKey, dataColumnSchema.ColumnName, columnSize, sb.ToString(),
                    dataColumnSchema.DataType);

                columnIndex++;
            }
        }

        return table;
    }

    private static string GetPrimaryKey(FoundationDbColumn dataColumnSchema)
    {
        var primaryKey = dataColumnSchema.IsKey == true
            ? "PKEY"
            : string.Empty;

        if (dataColumnSchema.IsIdentity == true)
        {
            if (primaryKey.Length > 0)
                primaryKey += ',';

            primaryKey += "IDENTITY";
        }

        return primaryKey;
    }

    List<Statement> IProvider.GetStatements(string commandText)
    {
        var sqlStatement = new SqlParser(commandText);
        var tokens = sqlStatement.Tokens;
        List<Statement> statements = [];

        IEnumerable<Token[]> statementTokenArrays = tokens.Split(token => IsBatchSeparator(commandText, token)).Where(statementTokens => statementTokens.Length > 0);

        foreach (Token[]? statementTokens in statementTokenArrays)
        {
            var startIndex = statementTokens[0].StartPosition;
            var endIndex = statementTokens.Last().EndPosition;
            var length = endIndex - startIndex + 1;
            var statement = new Statement(statementTokens[0].LineIndex, commandText.Substring(startIndex, length));
            statements.Add(statement);
        }

        return statements;
    }

    GetTableSchemaResult IProvider.GetTableSchema(IDbConnection connection, string? tableName) => TableSchema.GetTableSchema(connection, tableName);

    List<InfoMessage> IProvider.ToInfoMessages(Exception exception)
    {
        var now = LocalTime.Default.Now;
        List<InfoMessage> infoMessages;

        if (exception is AggregateException aggregateException)
            exception = UnAggregateException(aggregateException);

        if (exception is SqlException sqlException)
            infoMessages = ToInfoMessages(sqlException.Errors, now);
        else
        {
            var message = exception.ToLogString();
            var infoMessage = InfoMessageFactory.Create(InfoMessageSeverity.Error, null, message);
            infoMessages = [infoMessage];
        }

        return infoMessages;
    }

    private static Exception UnAggregateException(AggregateException aggregateException)
    {
        Exception unaggregatedException;
        if (aggregateException.InnerExceptions.Count == 1)
        {
            var innerException = aggregateException.InnerExceptions[0];
            if (innerException is AggregateException aggregateException2)
                unaggregatedException = UnAggregateException(aggregateException2);
            else
                unaggregatedException = innerException;
        }
        else
            unaggregatedException = aggregateException;

        return unaggregatedException;
    }

    private static object ConvertToString(object? source)
    {
        object target;
        if (source == null || source == DBNull.Value)
            target = DBNull.Value;
        else
        {
            var convertible = (IConvertible)source;
            target = convertible.ToString(null);
        }

        return target;
    }

    private static object ConvertToDecimal(object source)
    {
        object target;
        if (source == DBNull.Value)
            target = DBNull.Value;
        else
        {
            var decimalField = (DecimalField)source;
            target = decimalField.DecimalValue;
        }

        return target;
    }

    private static bool IsBatchSeparator(string commandText, Token token)
    {
        var isBatchSeparator =
            token.Type == TokenType.KeyWord &&
            string.Compare(token.Value, "GO", StringComparison.InvariantCultureIgnoreCase) == 0;

        if (isBatchSeparator)
        {
            var lineStartIndex = commandText.LastIndexOf('\n', token.StartPosition);
            lineStartIndex++;
            var lineEndIndex = commandText.IndexOf('\n', token.EndPosition + 1);
            if (lineEndIndex == -1) lineEndIndex = commandText.Length - 1;

            var lineLength = lineEndIndex - lineStartIndex + 1;
            var line = commandText.Substring(lineStartIndex, lineLength);
            line = line.Trim();
            isBatchSeparator = string.Compare(line, "GO", StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        return isBatchSeparator;
    }
}