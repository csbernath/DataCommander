namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    sealed class StoredProcedureCollectionNode : ITreeNode
    {
        public StoredProcedureCollectionNode(
            DatabaseNode database,
            bool isMSShipped)
        {
            this.database = database;
            this.isMSShipped = isMSShipped;
        }

        public string Name
        {
            get
            {
                return this.isMSShipped ? "System Stored Procedures" : "Stored Procedures";
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
            string commandText = string.Format( @"
select  s.name as Owner,
        o.name as Name        
from    [{0}].sys.all_objects o (readpast)
join    [{0}].sys.schemas s (readpast)
on      o.schema_id = s.schema_id
left join [{0}].sys.extended_properties p
on      o.object_id = p.major_id and p.minor_id = 0 and p.class = 1 and p.name = 'microsoft_database_tools_support'
where
    o.type = 'P'
    and o.is_ms_shipped = {1}
    and p.major_id is null
order by s.name,o.name", this.database.Name, this.isMSShipped ? 1 : 0 );

            DataTable dataTable;
            string connectionString = this.database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                dataTable = connection.ExecuteDataTable(commandText);
            }
            DataRowCollection dataRows = dataTable.Rows;
            int count = dataRows.Count;
            List<ITreeNode> treeNodes = new List<ITreeNode>();
            if (!this.isMSShipped)
            {
                treeNodes.Add(new StoredProcedureCollectionNode(this.database, true));
            }

            for (int i = 0; i < count; i++)
            {
                DataRow row = dataRows[i];
                string owner = (string)row["Owner"];
                string name = (string)row["Name"];

                treeNodes.Add(new StoredProcedureNode(this.database, owner, name));
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

        readonly DatabaseNode database;
        readonly bool isMSShipped;
    }
}