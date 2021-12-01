using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class TableCollectionNode : ITreeNode
    {
        public TableCollectionNode(DatabaseNode databaseNode) => DatabaseNode = databaseNode;

        public DatabaseNode DatabaseNode { get; }
        public string Name => "Tables";
        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var childNodes = new List<ITreeNode>();
            childNodes.Add(new SystemTableCollectionNode(DatabaseNode));

            var commandText = $@"select
    s.name,
    tbl.name,
    tbl.object_id
from [{DatabaseNode.Name}].sys.tables as tbl (nolock)
join [{DatabaseNode.Name}].sys.schemas s (nolock)
    on tbl.schema_id = s.schema_id
where
(CAST(
 case 
    when tbl.is_ms_shipped = 1 then 1
    when (
        select 
            major_id 
        from 
            [{DatabaseNode.Name}].sys.extended_properties 
        where 
            major_id = tbl.object_id and 
            minor_id = 0 and 
            class = 1 and 
            name = N'microsoft_database_tools_support') 
        is not null then 1
    else 0
end          
             AS bit)=0)
order by 1,2";
            var connectionString = DatabaseNode.Databases.Server.ConnectionString;
            SqlClientFactory.Instance.ExecuteReader(connectionString, new ExecuteReaderRequest(commandText), dataReader =>
            {
                while (dataReader.Read())
                {
                    var schema = dataReader.GetString(0);
                    var name = dataReader.GetString(1);
                    var objectId = dataReader.GetInt32(2);
                    var tableNode = new TableNode(DatabaseNode, schema, name, objectId);
                    childNodes.Add(tableNode);
                }
            });

            return childNodes;
        }

        public bool Sortable => false;
        public string Query => null;

        public ContextMenu GetContextMenu()
        {
            throw new System.NotImplementedException();
        }
    }
}