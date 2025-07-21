using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class DatabaseSnapshotCollectionNode(DatabaseCollectionNode databaseCollectionNode) : ITreeNode
{
    string? ITreeNode.Name => "Database Snapshots";
    bool ITreeNode.IsLeaf => false;

    async Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        const string commandText = @"select name
from sys.databases d
where
	d.source_database_id is not null
order by 1";
        return await Db.ExecuteReaderAsync(
            databaseCollectionNode.Server.CreateConnection,
            new ExecuteReaderRequest(commandText),
            128,
            ReadDatabaseNode,
            cancellationToken);
    }

    private DatabaseNode ReadDatabaseNode(IDataRecord dataRecord)
    {
        var name = dataRecord.GetString(0);
        return new DatabaseNode(databaseCollectionNode, name, 0);
    }

    bool ITreeNode.Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}