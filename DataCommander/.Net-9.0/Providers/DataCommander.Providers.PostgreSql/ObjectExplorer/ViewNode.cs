using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ViewNode(ViewCollectionNode viewCollectionNode, string? name) : ITreeNode
{
    private readonly ViewCollectionNode _viewCollectionNode = viewCollectionNode;
    private readonly string? _name = name;

    string? ITreeNode.Name => _name;
    bool ITreeNode.IsLeaf => true;
    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) => null;
    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}