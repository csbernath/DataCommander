namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class IndexCollectionNode : ITreeNode
    {
        private readonly TableNode tableNode;

        public IndexCollectionNode(TableNode tableNode)
        {
            this.tableNode = tableNode;
        }

        public string Name => "Indexes";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var cb = new SqlCommandBuilder();

            var commandText = string.Format(@"select
    i.name,
    i.type,
    i.is_unique    
from {0}.sys.schemas s (nolock)
join {0}.sys.objects o (nolock)
    on s.schema_id = o.schema_id
join {0}.sys.indexes i (nolock)
    on o.object_id = i.object_id
where o.object_id = {1}
order by i.name",
                cb.QuoteIdentifier(this.tableNode.DatabaseNode.Name),
                this.tableNode.Id);

            var connectionString = this.tableNode.DatabaseNode.Databases.Server.ConnectionString;
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