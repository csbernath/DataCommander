using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

internal sealed class SchemaNode(CatalogNode catalog, string name) : ITreeNode
{
    string? ITreeNode.Name
    {
        get
        {
            var name = Name;

            if (name == null)
                name = "[No schemas found]";

            return name;
        }
    }

    public bool IsLeaf => false;

    public IReadOnlyCollection<string> GetFilterableProperties() => [];

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(IReadOnlyList<FilterCriterion> filterCriteria, bool refresh, CancellationToken cancellationToken)
    {
        var treeNodes = new ITreeNode[2];
        treeNodes[0] = new TableCollectionNode(this);
        treeNodes[1] = new ProcedureCollectionNode(this);

        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }

    public bool Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public CatalogNode Catalog { get; } = catalog;
    public string Name { get; } = name;

    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();
}