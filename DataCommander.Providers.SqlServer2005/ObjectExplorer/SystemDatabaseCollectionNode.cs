namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class SystemDatabaseCollectionNode : ITreeNode
    {
        private readonly DatabaseCollectionNode databaseCollectionNode;

        public SystemDatabaseCollectionNode(DatabaseCollectionNode databaseCollectionNode)
        {
#if CONTRACTS_FULL
            Contract.Requires(databaseCollectionNode != null);
#endif
            this.databaseCollectionNode = databaseCollectionNode;
        }

#region ITreeNode Members

        string ITreeNode.Name => "System Databases";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var connectionString = this.databaseCollectionNode.Server.ConnectionString;
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

            var list = new List<ITreeNode>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var name = (string)dataRow[0];
                var node = new DatabaseNode(this.databaseCollectionNode, name);
                list.Add(node);
            }

            return list;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}