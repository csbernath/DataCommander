using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SchemaCollectionNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public SchemaCollectionNode(DatabaseNode database) => _database = database;

    public string Name => "Schemas";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        var treeNodes = await SqlClientFactory.Instance.ExecuteReaderAsync(
            _database.Databases.Server.ConnectionString,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
        return treeNodes;
    }

    private string CreateCommandText()
    {
        var commandText = @"select s.name
from {0}.sys.schemas s (nolock)
order by s.name";

        var sqlCommandBuilder = new SqlCommandBuilder();
        commandText = string.Format(commandText, sqlCommandBuilder.QuoteIdentifier(_database.Name));
        return commandText;
    }

    private SchemaNode ReadRecord(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        return new SchemaNode(_database, name);
    }

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}