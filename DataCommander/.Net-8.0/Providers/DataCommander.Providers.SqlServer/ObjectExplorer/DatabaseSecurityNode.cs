using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class DatabaseSecurityNode : ITreeNode
{
    private readonly DatabaseNode _databaseNode;

    public DatabaseSecurityNode(DatabaseNode databaseNode)
    {
        ArgumentNullException.ThrowIfNull(databaseNode);
        _databaseNode = databaseNode;
    }

    #region ITreeNode Members

    string ITreeNode.Name => "Security";

    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        return Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[]
        {
            new UserCollectionNode(_databaseNode),
            new RoleCollectionNode(_databaseNode),
            new SchemaCollectionNode(_databaseNode)
        });
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;

    #endregion
}