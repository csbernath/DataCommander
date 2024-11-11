using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SystemViewCollectionNode(DatabaseNode databaseNode) : ITreeNode
{
    public string? Name => "System Views";
    public bool IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var commandText = CreateCommandText();
        return await Db.ExecuteReaderAsync(
            databaseNode.Databases.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadRecord,
            cancellationToken);
    }

    private string CreateCommandText()
    {
        var cb = new SqlCommandBuilder();
        var databaseName = cb.QuoteIdentifier(databaseNode.Name);
        var commandText = $@"select
    s.name,
    v.name,
    v.object_id
from {databaseName}.sys.schemas s (nolock)
join {databaseName}.sys.system_views v (nolock)
    on s.schema_id = v.schema_id
order by 1,2";
        commandText = string.Format(commandText, databaseNode.Name);
        return commandText;
    }

    private ViewNode ReadRecord(IDataRecord dataRecord)
    {
        var schema = dataRecord.GetString(0);
        var name = dataRecord.GetString(1);
        var id = dataRecord.GetInt32(2);
        return new ViewNode(databaseNode, id, schema, name);
    }

    public bool Sortable => false;
    public string? Query => null;

    public ContextMenu? GetContextMenu() => null;
}