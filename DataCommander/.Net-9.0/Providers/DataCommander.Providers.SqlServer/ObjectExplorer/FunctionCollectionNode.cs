using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class FunctionCollectionNode(DatabaseNode database) : ITreeNode
{
    public string? Name => "Functions";
    public bool IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[]
        {
            new TableValuedFunctionCollectionNode(database),
            new ScalarValuedFunctionCollectionNode(database)
        });

    public bool Sortable => false;
    public string Query => null;

    public ContextMenu? GetContextMenu() => null;
}