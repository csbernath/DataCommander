using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Api;

public interface ITreeNode
{
    string? Name { get; }
    bool IsLeaf { get; }

    IReadOnlyCollection<string> GetFilterableProperties();
    Task<IEnumerable<ITreeNode>> GetChildren(IReadOnlyCollection<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken);
    IReadOnlyCollection<FilterCriterion> GetFilterCriteria();
    
    bool Sortable { get; }
    bool DynamicChildCount { get; }
    Task<string?> GetQuery(CancellationToken cancellationToken);
    ContextMenu? GetContextMenu();
}