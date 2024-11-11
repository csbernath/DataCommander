using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.OleDb;

/// <summary>
/// Summary description for CatalogsNode.
/// </summary>
sealed class SchemaNode(CatalogNode catalog, string name) : ITreeNode
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

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var treeNodes = new ITreeNode[2];
        treeNodes[0] = new TableCollectionNode(this);
        treeNodes[1] = new ProcedureCollectionNode(this);

        return Task.FromResult<IEnumerable<ITreeNode>>(treeNodes);
    }

    public bool Sortable => false;
    public string? Query => null;
    public CatalogNode Catalog { get; } = catalog;
    public string Name { get; } = name;

    public ContextMenu? GetContextMenu() => throw new System.NotImplementedException();
}