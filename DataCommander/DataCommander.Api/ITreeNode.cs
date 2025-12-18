using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Api;

public interface ITreeNode
{
    string? Name { get; }
    bool IsLeaf { get; }

    IReadOnlyCollection<string> GetFilterableProperties();
    Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken);
    bool Sortable { get; }
    bool DynamicChildCount { get; }
    Task<string?> GetQuery(CancellationToken cancellationToken);
    ContextMenu? GetContextMenu();
}