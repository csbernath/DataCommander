using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class FunctionCollectionNode(DatabaseNode database) : ITreeNode
{
    public string? Name => "Functions";
    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    IAsyncEnumerable<ITreeNode> ITreeNode.GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        ITreeNode[] children =
        [
            new TableValuedFunctionCollectionNode(database),
            new ScalarValuedFunctionCollectionNode(database)
        ];
        return children.ToAsyncEnumerable();
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    public bool Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}