using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SQLite.ObjectExplorer;

sealed class DatabaseNode(DatabaseCollectionNode databaseCollectionNode, string? name) : ITreeNode
{
    public DatabaseCollectionNode DatabaseCollectionNode => databaseCollectionNode;

    #region ITreeNode Members
    public string? Name { get; } = name;

    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public async IAsyncEnumerable<ITreeNode> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        yield return new TableCollectionNode(this);
    }

    public IReadOnlyCollection<FilterCriterion> GetFilterCriteria() => [];

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();

    #endregion
}