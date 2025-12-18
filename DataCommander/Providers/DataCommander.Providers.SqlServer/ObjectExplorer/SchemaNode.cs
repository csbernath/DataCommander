using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class SchemaNode(DatabaseNode database, string? name) : ITreeNode
{
    private readonly DatabaseNode _database = database;

    public string? Name { get; } = name;
    public bool IsLeaf => true;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}