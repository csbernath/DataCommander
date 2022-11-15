using System;
using System.Collections.Generic;
using DataCommander.Api;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LinkedServerCollectionNode : ITreeNode
{
    public LinkedServerCollectionNode(ServerNode serverNode)
    {
        ArgumentNullException.ThrowIfNull(serverNode);
        Server = serverNode;
    }

    public ServerNode Server { get; }

    #region ITreeNode Members

    string ITreeNode.Name => "Linked Servers";

    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        const string commandText = @"select  s.name
from    sys.servers s (nolock)
where   s.is_linked = 1
order by s.name";

        using (var connection = new SqlConnection(Server.ConnectionString))
        {
            connection.Open();
            var executor = connection.CreateCommandExecutor();
            return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataReader =>
            {
                var name = dataReader.GetString(0);
                return (ITreeNode) new LinkedServerNode(this, name);
            });
        }
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;

    #endregion
}