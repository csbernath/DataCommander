using System;
using Foundation.Data;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    internal sealed class LinkedServerCollectionNode : ITreeNode
    {
        public LinkedServerCollectionNode( ServerNode serverNode )
        {
            FoundationContract.Requires<ArgumentException>( serverNode != null );

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

            List<ITreeNode> treeNodes;

            using (var connection = new SqlConnection(Server.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                treeNodes = executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
                {
                    var name = dataReader.GetString(0);
                    return (ITreeNode) new LinkedServerNode(this, name);
                });
            }

            return treeNodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}