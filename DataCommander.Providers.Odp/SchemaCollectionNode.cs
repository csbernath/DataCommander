namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;
    using Oracle.ManagedDataAccess.Client;

    /// <summary>
    /// Summary description for SchemaNode.
    /// </summary>
    internal sealed class SchemaCollectionNode : ITreeNode
    {
        public SchemaCollectionNode(OracleConnection connection)
        {
            this.connection = connection;
            OracleConnectionStringBuilder oracleConnectionStringBuilder = new OracleConnectionStringBuilder(connection.ConnectionString);
            this.selectedSchema = oracleConnectionStringBuilder.UserID;
        }

        public string Name
        {
            get
            {
                return "Schemas";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            string commandText = "select username from all_users order by username";
            var transactionScope = new DbTransactionScope(this.connection, null);
            DataTable dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            int count = dataTable.Rows.Count;
            var treeNodes = new ITreeNode[count];

            for (int i = 0; i < count; i++)
            {
                string name = (string)dataTable.Rows[i][0];
                treeNodes[i] = new SchemaNode(this, name);
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

        public OracleConnection Connection
        {
            get
            {
                return connection;
            }
        }

        public void BeforeExpand()
        {
        }

        public string SelectedSchema
        {
            get
            {
                return selectedSchema;
            }
            set
            {
                selectedSchema = value;
            }
        }

        readonly OracleConnection connection;
        string selectedSchema;
    }
}