using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class DatabaseCollectionNode(ObjectExplorer objectExplorer) : ITreeNode
{
    public ObjectExplorer ObjectExplorer { get; } = objectExplorer;
    bool ITreeNode.IsLeaf => false;
    string ITreeNode.Name => "Databases";

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    bool ITreeNode.Sortable => false;

    public async Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        const string commandText = @"select datname
from pg_database
order by 1";
        var databaseNodes = await Db.ExecuteReaderAsync(
            () => ObjectExplorer.CreateConnection(),
            new ExecuteReaderRequest(commandText),
            128,
            dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return new DatabaseNode(this, name);
            },
            cancellationToken);

        return databaseNodes;
    }
}