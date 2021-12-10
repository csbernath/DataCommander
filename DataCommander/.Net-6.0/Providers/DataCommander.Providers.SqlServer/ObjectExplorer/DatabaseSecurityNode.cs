using System.Collections.Generic;
using DataCommander.Providers2;
using Foundation.Assertions;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class DatabaseSecurityNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;

    public DatabaseSecurityNode(DatabaseNode databaseNode)
    {
        Assert.IsNotNull(databaseNode);
        _databaseNode = databaseNode;
    }

    #region ITreeNode Members

    string ITreeNode.Name => "Security";

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return new ITreeNode[]
        {
            new UserCollectionNode(_databaseNode),
            new RoleCollectionNode(_databaseNode),
            new SchemaCollectionNode(_databaseNode)
        };
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu() => null;

    #endregion
}