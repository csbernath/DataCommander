namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class SystemDatabaseCollectionNode : ITreeNode
    {
        private readonly DatabaseCollectionNode databaseCollectionNode;

        public SystemDatabaseCollectionNode(DatabaseCollectionNode databaseCollectionNode)
        {
            Contract.Requires(databaseCollectionNode != null);
            this.databaseCollectionNode = databaseCollectionNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "System Databases";
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
            string connectionString = this.databaseCollectionNode.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection(connectionString))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                const string commandText = @"select d.name
from sys.databases d (nolock)
where name in('master','model','msdb','tempdb')
order by d.name";
                dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            }

            List<ITreeNode> list = new List<ITreeNode>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string name = (string)dataRow[0];
                DatabaseNode node = new DatabaseNode(this.databaseCollectionNode, name);
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