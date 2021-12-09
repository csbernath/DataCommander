using System.Collections.Generic;

namespace DataCommander.Providers.MySql.ObjectExplorer;

internal sealed class DatabaseNode : ITreeNode
{
    public DatabaseNode(ObjectExplorer objectExplorer, string name)
    {
        ObjectExplorer = objectExplorer;
        Name = name;
    }

    public ObjectExplorer ObjectExplorer { get; }

    public string Name { get; }

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return new ITreeNode[]
        {
            new TableCollectionNode(this),
            new ViewCollectionNode(this),
            new StoredProcedureCollectionNode(this),
            new FunctionCollectionNode(this),
        };
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu() => null;
}