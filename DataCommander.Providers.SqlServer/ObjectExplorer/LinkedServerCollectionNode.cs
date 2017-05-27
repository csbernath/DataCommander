using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;

    internal sealed class LinkedServerCollectionNode : ITreeNode
    {
        public LinkedServerCollectionNode( ServerNode serverNode )
        {
#if CONTRACTS_FULL
            Contract.Requires( serverNode != null );
#endif
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
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    treeNodes = dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        return (ITreeNode)new LinkedServerNode(this, name);
                    }).ToList();
                }
            }

            return treeNodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}