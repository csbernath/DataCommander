using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

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
    s.name,
    tbl.name,
    tbl.object_id
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
            using (var connection = new SqlConnection(connectionString))
            {
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        var schema = dataRecord.GetString(0);
                        var name = dataRecord.GetString(1);
                        var id = dataRecord.GetInt32(2);
                        var tableNode = new TableNode(this.DatabaseNode, schema, name, id);
                        childNodes.Add(tableNode);
                    });
                }
            }
            return childNodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        public DatabaseNode DatabaseNode { get; }

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}