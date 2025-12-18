using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ProgrammabilityNode(DatabaseNode database) : ITreeNode
{
    string? ITreeNode.Name => "Programmability";
    bool ITreeNode.IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        List<ITreeNode> childNodes =
        [
            new StoredProcedureCollectionNode(database, false),
            new FunctionCollectionNode(database)
        ];

        if (database.Name == "master")
            childNodes.Add(new ExtendedStoreProcedureCollectionNode(database));

        childNodes.Add(new UserDefinedTableTypeCollectionNode(database));

        return Task.FromResult<IEnumerable<ITreeNode>>(childNodes);
    }

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);

    public ContextMenu? GetContextMenu() => null;
}