namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class LinkedServerCollectionNode : ITreeNode
    {
        private readonly ServerNode server;

        public LinkedServerCollectionNode( ServerNode serverNode )
        {
            Contract.Requires( serverNode != null );
            this.server = serverNode;
        }

        public ServerNode Server
        {
            get
            {
                return this.server;
            }
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Linked Servers";
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            const string commandText = @"select  s.name
from    sys.servers s (nolock)
where   s.is_linked = 1
order by s.name";

            using (var connection = new SqlConnection(this.server.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText},CommandBehavior.Default))
                {
                    return dataReader.Read(dataRecord =>
                    {
                        string name = dataRecord.GetString(0);
                        return new LinkedServerNode(this, name);
                    });
                }
            }
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return null;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}