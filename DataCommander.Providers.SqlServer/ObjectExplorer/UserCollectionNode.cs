namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class UserCollectionNode : ITreeNode
    {
        private readonly DatabaseNode database;

        public UserCollectionNode(DatabaseNode  database)
        {
            this.database = database;
        }

        public string Name => "Users";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = "select name from {0}..sysusers where islogin = 1 order by name";
            commandText = string.Format(commandText, this.database.Name);
            var connectionString = this.database.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection(connectionString))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            }
            var dataRows = dataTable.Rows;
            var count = dataRows.Count;
            var treeNodes = new ITreeNode[count];

            for (var i=0;i<count;i++)
            {
                var name = (string)dataRows[i][0];
                treeNodes[i] = new UserNode(this.database,name);
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;
    }
}