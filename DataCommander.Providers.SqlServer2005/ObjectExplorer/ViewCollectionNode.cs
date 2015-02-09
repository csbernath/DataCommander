namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class ViewCollectionNode : ITreeNode
    {
        public ViewCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        public string Name
        {
            get
            {
                return "Views";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            string commandText = @"select
     s.name as SchemaName
    ,v.name as ViewName
from [{0}].sys.views v (nolock)
join [{0}].sys.schemas s (nolock)
    on v.schema_id = s.schema_id
order by s.name,v.name";
            commandText = string.Format(commandText, database.Name);
            string connectionString = this.database.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection(connectionString))
            {
                dataTable = connection.ExecuteDataTable(null, commandText, CommandType.Text, 0);
            }
            DataRowCollection dataRows = dataTable.Rows;
            int count = dataRows.Count;
            List<ITreeNode> treeNodes = new List<ITreeNode>();
            treeNodes.Add(new SystemViewCollectionNode(this.database));

            for (int i = 0; i < count; i++)
            {
                DataRow row = dataRows[i];
                string schema = (string)row[0];
                string name = (string)row[1];
                treeNodes.Add(new ViewNode(database, schema, name));
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

        private DatabaseNode database;
    }
}