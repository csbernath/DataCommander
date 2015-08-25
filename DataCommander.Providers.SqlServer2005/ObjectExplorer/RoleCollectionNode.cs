namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class RoleCollectionNode : ITreeNode
    {
        public RoleCollectionNode( DatabaseNode database )
        {
            this.database = database;
        }

        public string Name
        {
            get
            {
                return "Roles";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            string commandText = "select name from {0}..sysusers where issqlrole = 1 order by name";
            commandText = string.Format( commandText, this.database.Name );
            string connectionString = this.database.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection( connectionString ))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            }
            DataRowCollection dataRows = dataTable.Rows;
            int count = dataRows.Count;
            ITreeNode[] treeNodes = new ITreeNode[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string) dataRows[ i ][ 0 ];
                treeNodes[ i ] = new RoleNode(this.database, name );
            }

            return treeNodes;
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        public string Query
        {
            get
            {
                return null;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                return null;
            }
        }

        private readonly DatabaseNode database;
    }
}