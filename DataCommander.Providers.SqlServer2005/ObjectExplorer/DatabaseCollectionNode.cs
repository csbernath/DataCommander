namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class DatabaseCollectionNode : ITreeNode
    {
        private ServerNode server;

        public DatabaseCollectionNode(ServerNode server)
        {
            Contract.Requires(server != null);
            this.server = server;
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
                return "Databases";
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
            string connectionString = this.server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection(connectionString))
            {
                const string commandText = @"select d.name
from sys.databases d (nolock)
where name not in('master','model','msdb','tempdb')
order by d.name";
                dataTable = connection.ExecuteDataTable(commandText);
            }

            List<ITreeNode> list = new List<ITreeNode>();
            list.Add( new SystemDatabaseCollectionNode( this ) );

            foreach (DataRow dataRow in dataTable.Rows)
            {
                string name = (string)dataRow[0];
                DatabaseNode node = new DatabaseNode(this, name);
                list.Add(node);
            }

            return list;
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