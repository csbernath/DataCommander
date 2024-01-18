using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using DataCommander.Api.Connection;
using Microsoft.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ServerNode : ITreeNode
{
    public readonly ConnectionStringAndCredential ConnectionStringAndCredential;
    
    public ServerNode(ConnectionStringAndCredential connectionStringAndCredential)
    {
        ConnectionStringAndCredential = connectionStringAndCredential;
    }

    public SqlConnection CreateConnection() => ConnectionFactory.CreateConnection(ConnectionStringAndCredential);

    #region ITreeNode Members

    string ITreeNode.Name => ConnectionNameProvider.GetConnectionName(CreateConnection);

    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var node = new DatabaseCollectionNode(this);
        var securityNode = new SecurityNode(this);
        var serverObjectCollectionNode = new ServerObjectCollectionNode(this);
        var jobCollectionNode = new JobCollectionNode(this);
        return Task.FromResult<IEnumerable<ITreeNode>>(new ITreeNode[] { node, securityNode, serverObjectCollectionNode, jobCollectionNode });
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;

    #endregion
}