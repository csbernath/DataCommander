using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    internal sealed class ViewCollectionNode : ITreeNode
    {
        public ViewCollectionNode(DatabaseNode database)
        {
            _database = database;
        }

        public string Name => "Views";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var treeNodes = new List<ITreeNode>();
            treeNodes.Add(new SystemViewCollectionNode(_database));

            var databaseName = new SqlCommandBuilder().QuoteIdentifier(_database.Name);
            var commandText = $@"select
    s.name,
    v.name,
    v.object_id
from {databaseName}.sys.schemas s (nolock)
join {databaseName}.sys.views v (nolock)
    on s.schema_id = v.schema_id
order by 1,2";
            SqlClientFactory.Instance.ExecuteReader(_database.Databases.Server.ConnectionString, new ExecuteReaderRequest(commandText), dataReader =>
            {
                dataReader.ReadResult(() =>
                {
                    var schema = dataReader.GetString(0);
                    var name = dataReader.GetString(1);
                    var id = dataReader.GetInt32(2);
                    treeNodes.Add(new ViewNode(_database, id, schema, name));
                });
            });
            return treeNodes;
        }

        public bool Sortable => false;
        public string Query => null;
        public ContextMenuStrip ContextMenu => null;
        private readonly DatabaseNode _database;
    }
}