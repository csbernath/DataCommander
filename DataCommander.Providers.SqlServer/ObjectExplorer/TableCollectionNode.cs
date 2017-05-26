using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    internal sealed class TableCollectionNode : ITreeNode
    {
        public TableCollectionNode( DatabaseNode databaseNode )
        {
            this.DatabaseNode = databaseNode;
        }

        public string Name => "Tables";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
//            string commandText = @"select	u.name,o.name
//from	{0}.dbo.sysobjects o
//join	{0}.dbo.sysusers u
//on	o.uid	= u.uid
//where	o.type='{1}'
//order by u.name,o.name";
//            string databaseName = databaseNode.Name;
//            commandText = string.Format( commandText, databaseName, type );
//            DataTable dataTable = new DataTable();
//            SqlDatabase.Fill( commandText, databaseNode.Connection, dataTable );
//            int count = dataTable.Rows.Count;
//            ITreeNode[] treeNodes = new ITreeNode[ count ];

//            for (int i = 0; i < count; i++)
//            {
//                DataRow row = dataTable.Rows[ i ];
//                string owner = (string) row[ 0 ];
//                string name = (string) row[ 1 ];
//                treeNodes[ i ] = new TableNode( databaseNode, owner, name );
//            }

//            return treeNodes;

            var childNodes = new List<ITreeNode>();
            childNodes.Add(new SystemTableCollectionNode(this.DatabaseNode));

            var commandText = string.Format(@"select
    s.name,
    tbl.name,
    tbl.object_id
from [{0}].sys.tables as tbl (nolock)
join [{0}].sys.schemas s (nolock)
    on tbl.schema_id = s.schema_id
where
(CAST(
 case 
    when tbl.is_ms_shipped = 1 then 1
    when (
        select 
            major_id 
        from 
            [{0}].sys.extended_properties 
        where 
            major_id = tbl.object_id and 
            minor_id = 0 and 
            class = 1 and 
            name = N'microsoft_database_tools_support') 
        is not null then 1
    else 0
end          
             AS bit)=0)
order by 1,2", this.DatabaseNode.Name);
            var connectionString = this.DatabaseNode.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        var schema = dataRecord.GetString(0);
                        var name = dataRecord.GetString(1);
                        var object_id = dataRecord.GetInt32(2);
                        var tableNode = new TableNode(this.DatabaseNode, schema, name, object_id);
                        childNodes.Add(tableNode);
                    });
                }
            }
            return childNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public DatabaseNode DatabaseNode { get; }

        public ContextMenuStrip ContextMenu => null;
    }
}