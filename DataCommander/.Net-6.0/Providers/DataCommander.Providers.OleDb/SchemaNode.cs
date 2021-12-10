using System.Collections.Generic;
using DataCommander.Providers2;

namespace DataCommander.Providers.OleDb;

/// <summary>
/// Summary description for CatalogsNode.
/// </summary>
sealed class SchemaNode : ITreeNode
{
    public SchemaNode(CatalogNode catalog, string name)
    {
        Catalog = catalog;
        Name = name;
    }

    string ITreeNode.Name
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

    public IEnumerable<ITreeNode> GetChildren(bool refresh)
    {
        var treeNodes = new ITreeNode[2];
        treeNodes[0] = new TableCollectionNode(this);
        treeNodes[1] = new ProcedureCollectionNode(this);

        return treeNodes;
    }

    public bool Sortable => false;
    public string Query => null;
    public CatalogNode Catalog { get; }
    public string Name { get; }

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }
}