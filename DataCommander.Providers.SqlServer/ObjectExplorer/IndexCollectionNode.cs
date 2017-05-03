namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using Foundation.Data;
    using Foundation.Data.SqlClient;

    internal sealed class IndexCollectionNode : ITreeNode
    {
        private readonly TableNode tableNode;
        private readonly string schema;
        private readonly string objectName;

        public IndexCollectionNode(TableNode tableNode, string owner, string objectName)
        {
            this.tableNode = tableNode;
            this.schema = owner;
            this.objectName = objectName;
        }

        public string Name => "Indexes";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var cb = new SqlCommandBuilder();
            var tableName = new DatabaseObjectMultipartName(null, this.tableNode.Database.Name, this.schema, this.objectName);

            var commandText = string.Format(@"select
     i.name
    ,i.type
    ,i.is_unique    
from {0}.sys.schemas s (nolock)
join {0}.sys.objects o (nolock)
    on s.schema_id = o.schema_id
join {0}.sys.indexes i (nolock)
    on o.object_id = i.object_id
where
    s.name = {1}
    and o.name = {2}
order by i.name",
                cb.QuoteIdentifier(this.tableNode.Database.Name),
                tableName.Schema.ToTSqlNVarChar(),
                tableName.Name.ToTSqlNVarChar());

            var connectionString = this.tableNode.Database.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    return dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var type = dataRecord.GetByte(1);
                        var isUnique = dataRecord.GetBoolean(2);
                        return new IndexNode(this.tableNode, name, type, isUnique);
                    }).ToList();
                }
            }
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;
    }
}