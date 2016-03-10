namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class SystemTableCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public SystemTableCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        string ITreeNode.Name => "System Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            List<ITreeNode> childNodes = new List<ITreeNode>();
            string commandText = string.Format( @"select
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
order by [Schema],[Name]", this.databaseNode.Name);
            string connectionString = this.DatabaseNode.Databases.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection(connectionString))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
            }
            foreach (DataRow dataRow in dataTable.Rows)
            {
                String schema = (String)dataRow["Schema"];
                String name = (String)dataRow["Name"];
                TableNode tableNode = new TableNode(this.databaseNode, schema, name);
                childNodes.Add(tableNode);
            }

            return childNodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        public DatabaseNode DatabaseNode => this.databaseNode;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}