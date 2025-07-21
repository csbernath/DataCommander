using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Api;

public interface ITreeNode
{
    string? Name { get; }
    bool IsLeaf { get; }
    Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken);
    bool Sortable { get; }
    Task<string?> GetQuery(CancellationToken cancellation);
    ContextMenu? GetContextMenu();
}