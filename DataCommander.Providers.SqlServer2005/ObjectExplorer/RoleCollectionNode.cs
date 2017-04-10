namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class RoleCollectionNode : ITreeNode
    {
        public RoleCollectionNode( DatabaseNode database )
        {
            this.database = database;
        }

        public string Name => "Roles";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            var commandText = "select name from {0}..sysusers where issqlrole = 1 order by name";
            commandText = string.Format( commandText, this.database.Name );
            var connectionString = this.database.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection( connectionString ))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            }
            var dataRows = dataTable.Rows;
            var count = dataRows.Count;
            var treeNodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var name = (string) dataRows[ i ][ 0 ];
                treeNodes[ i ] = new RoleNode(this.database, name );
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        private readonly DatabaseNode database;
    }
}