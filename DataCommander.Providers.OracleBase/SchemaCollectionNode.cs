namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    /// <summary>
    /// Summary description for SchemaNode.
    /// </summary>
    public sealed class SchemaCollectionNode : ITreeNode
    {
        private IDbConnection connection;
        private string selectedSchema;

        public SchemaCollectionNode(IDbConnection connection)
        {
            this.connection = connection;
            DbConnectionStringBuilder sb = new DbConnectionStringBuilder();
            sb.ConnectionString = connection.ConnectionString;
            this.selectedSchema = (string)sb[ ConnectionStringProperty.UserId ];
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
            DataTable dataTable = connection.ExecuteDataTable( null, commandText, CommandType.Text, 0 );
            int count = dataTable.Rows.Count;

            ITreeNode[] treeNodes = new ITreeNode[count];

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

        public IDbConnection Connection
        {
            get
            {
                return this.connection;
            }
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
    }
}