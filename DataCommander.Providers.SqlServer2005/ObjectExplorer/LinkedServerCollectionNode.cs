namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class LinkedServerCollectionNode : ITreeNode
    {
        public LinkedServerCollectionNode( ServerNode serverNode )
        {
            Contract.Requires( serverNode != null );
            this.Server = serverNode;
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

            using (var connection = new SqlConnection(this.Server.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    treeNodes = dataReader.Read(dataRecord =>
                    {
                        string name = dataRecord.GetString(0);
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