using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string? name) : ITreeNode
{
    public DatabaseCollectionNode DatabaseCollectionNode { get; } = databaseCollectionNode;

    public string? Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<ITreeNode>>(
            new ITreeNode[]
            {
                new SchemaCollectionNode(this)
            });
    }

    bool ITreeNode.Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;

    public NpgsqlConnection CreateConnection() => DatabaseCollectionNode.ObjectExplorer.CreateConnection(name);
}