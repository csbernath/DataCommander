namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System.Collections.Generic;
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
            var oracleConnectionStringBuilder = new OracleConnectionStringBuilder(connection.ConnectionString);
            this.selectedSchema = oracleConnectionStringBuilder.UserID;
        }

        public string Name => "Schemas";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var commandText = "select username from all_users order by username";
            var transactionScope = new DbTransactionScope(this.connection, null);
            var dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            var count = dataTable.Rows.Count;
            var treeNodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var name = (string)dataTable.Rows[i][0];
                treeNodes[i] = new SchemaNode(this, name);
            }

            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        public OracleConnection Connection => connection;

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