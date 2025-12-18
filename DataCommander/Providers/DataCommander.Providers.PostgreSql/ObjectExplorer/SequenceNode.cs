using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class SequenceNode(SequenceCollectionNode sequenceCollectionNode, string? name) : ITreeNode
{
    private readonly SequenceCollectionNode _sequenceCollectionNode = sequenceCollectionNode;

    string? ITreeNode.Name => name;

    bool ITreeNode.IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    public Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}