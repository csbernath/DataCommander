using System;
using Foundation.Data;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;

    internal sealed class SystemDatabaseCollectionNode : ITreeNode
    {
        private readonly DatabaseCollectionNode _databaseCollectionNode;

        public SystemDatabaseCollectionNode(DatabaseCollectionNode databaseCollectionNode)
        {
            FoundationContract.Requires<ArgumentException>(databaseCollectionNode != null);

            _databaseCollectionNode = databaseCollectionNode;
        }

#region ITreeNode Members

        string ITreeNode.Name => "System Databases";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var connectionString = _databaseCollectionNode.Server.ConnectionString;
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
                var node = new DatabaseNode(_databaseCollectionNode, name);
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