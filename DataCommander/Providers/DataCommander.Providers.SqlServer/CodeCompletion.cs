using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using DataCommander.Api.Query;
using Foundation.Data;
using Foundation.Log;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer;

internal static class CodeCompletion
{
    private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
    
    public static string? GetTableViewFunctionCommandText(SqlObject sqlObject)
    {
        string? commandText;
        var nameParts = sqlObject.Name != null
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

                var objectTypes = sqlObject.Type.ToObjectTypes();
                statements.Add(SqlServerObject.GetObjects("dbo", objectTypes));
            }
                break;

            case 2:
                if (nameParts![0] != null)
                {
                    statements.Add(SqlServerObject.GetSchemas(nameParts[0]!));

                    var objectTypes = sqlObject.Type.ToObjectTypes();
                    statements.Add(SqlServerObject.GetObjects(nameParts[0]!, objectTypes));
                }

                break;

            case 3:
            {
                if (nameParts![0] != null && nameParts[1] != null)
                {
                    var objectTypes = sqlObject.Type.ToObjectTypes();
                    statements.Add(SqlServerObject.GetObjects(nameParts[0]!, nameParts[1]!, objectTypes));
                }
            }
                break;
        }

        commandText = statements.Count > 0
            ? string.Join("\r\n", statements)
            : null;
        return commandText;
    }

    public static string GetColumnCommandText(ConnectionBase connection, SqlObject sqlObject)
    {
        DatabaseObjectMultipartName name;
        int i;
        string commandText;
        name = new DatabaseObjectMultipartName(connection.Database, sqlObject.ParentName);
        string?[] owners;

        if (name.Schema != null)
            owners = [name.Schema];
        else
            owners = ["dbo", "sys"];

        var stringBuilder = new StringBuilder();
        for (i = 0; i < owners.Length; i++)
        {
            if (i > 0) stringBuilder.Append(',');

            stringBuilder.AppendFormat("'{0}'", owners[i]);
        }

        var ownersString = stringBuilder.ToString();
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
        return commandText;
    }

    public static string GetProcedureCommandText(ConnectionBase connection, SqlObject sqlObject)
    {
        var name = new DatabaseObjectMultipartName(connection.Database, sqlObject.Name);
        name.Schema ??= "dbo";
        var commandText = SqlServerObject.GetObjectsByDatabase(name.Database!, ["P", "X"]);
        return commandText;
    }

    public static string? GetValueCommandText(SqlObject sqlObject, SqlParser sqlStatement, Token? previousToken, IReadOnlyList<Token> tokens)
    {
        string? commandText = null;        
        int i;
        var items = sqlObject.ParentName!.Split('.');
        i = items.Length - 1;
        var sqlCommandBuilder = new SqlCommandBuilder();
        var columnName = sqlCommandBuilder.QuoteIdentifier(items[i]);

        string? tableNameOrAlias = null;
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
                string? where;
                var tokenIndex = previousToken!.Index + 1;
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

        return commandText;
    }

    public static async Task<List<IObjectName>?> GetObjectNames(
        ConnectionBase connection,
        IDbTransaction transaction,
        string? commandText,
        CancellationToken cancellationToken)
    {
        List<IObjectName>? array = null;        
        if (commandText != null)
        {
            Log.Write(LogLevel.Trace, "commandText:\r\n{0}", commandText);
            List<IObjectName> list = [];
            try
            {
                if (connection.State != ConnectionState.Open)
                    await connection.OpenAsync(cancellationToken);

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
                                objectName = dataReader[0].ToString()!;
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

        return array;
    }
}