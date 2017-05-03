namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class SystemTableCollectionNode : ITreeNode
    {
        public SystemTableCollectionNode(DatabaseNode databaseNode)
        {
            this.DatabaseNode = databaseNode;
        }

        string ITreeNode.Name => "System Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var childNodes = new List<ITreeNode>();
            var commandText = string.Format( @"select
    s.name as [Schema],
    tbl.name AS [Name]
from [{0}].sys.tables AS tbl
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
             AS bit)=1)
order by [Schema],[Name]", this.DatabaseNode.Name);
            var connectionString = this.DatabaseNode.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection(connectionString))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            }
            foreach (DataRow dataRow in dataTable.Rows)
            {
                var schema = (string)dataRow["Schema"];
                var name = (string)dataRow["Name"];
                var tableNode = new TableNode(this.DatabaseNode, schema, name);
                childNodes.Add(tableNode);
            }

            return childNodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        public DatabaseNode DatabaseNode { get; }

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}