using Foundation.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data.SqlClient;
using SqlCommandExecutor = Foundation.Data.SqlClient2.SqlCommandExecutor;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class IndexCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;
        private readonly int _id;

        public IndexCollectionNode(DatabaseNode databaseNode, int id)
        {
            _databaseNode = databaseNode;
            _id = id;
        }

        public string Name => "Indexes";
        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var cb = new SqlCommandBuilder();

            var commandText = string.Format(@"select
    i.name,
    i.index_id,
    i.type,
    i.is_unique
from {0}.sys.schemas s (nolock)
join {0}.sys.objects o (nolock)
    on s.schema_id = o.schema_id
join {0}.sys.indexes i (nolock)
    on o.object_id = i.object_id
where
    o.object_id = @object_id and
    i.type > 0
order by i.name",
                cb.QuoteIdentifier(_databaseNode.Name));

            var parameters = new SqlParameterCollectionBuilder();
            parameters.Add("object_id", _id);
            var request = new ExecuteReaderRequest(commandText, parameters.ToReadOnlyCollection());

            var connectionString = _databaseNode.Databases.Server.ConnectionString;
            var executor = new SqlCommandExecutor(connectionString);

            return executor.ExecuteReader(request, 128, dataRecord =>
            {
                var name = dataRecord.GetStringOrDefault(0);
                var indexId = dataRecord.GetInt32(1);
                var type = dataRecord.GetByte(2);
                var isUnique = dataRecord.GetBoolean(3);
                return new IndexNode(_databaseNode, _id, indexId, name, type, isUnique);
            });
        }

        public bool Sortable => false;
        public string Query => null;
        public ContextMenuStrip ContextMenu => null;
    }
}