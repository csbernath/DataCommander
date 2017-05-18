namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using Foundation.Data;
    using Foundation.Data.SqlClient;
    using Foundation.Threading.Tasks;

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
where o.object_id = @object_id
order by i.name",
                cb.QuoteIdentifier(this.tableNode.DatabaseNode.Name));

            var parameters = new List<SqlParameter>();
            parameters.Add("object_id", this.tableNode.Id);
            var request = new ExecuteReaderRequest(commandText, parameters);

            var connectionString = this.tableNode.DatabaseNode.Databases.Server.ConnectionString;
            var dbContext = new SqlConnectionStringDbContext(connectionString);

            var response = TaskSyncRunner.Run(() => dbContext.ExecuteReaderAsync(request, dataRecord =>
            {
                var name = dataRecord.GetString(0);
                var type = dataRecord.GetByte(1);
                var isUnique = dataRecord.GetBoolean(2);
                return new IndexNode(this.tableNode, name, type, isUnique);
            }));

            return response.Rows;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;
    }
}