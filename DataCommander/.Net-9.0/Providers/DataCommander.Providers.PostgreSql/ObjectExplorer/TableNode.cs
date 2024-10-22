using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class TableNode(TableCollectionNode tableCollectionNode, string? name) : ITreeNode
{
    public TableCollectionNode TableCollectionNode { get; } = tableCollectionNode;

    public string? Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(
        [
            new ColumnCollectionNode(this)
        ]);

    bool ITreeNode.Sortable => false;
    string? ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}