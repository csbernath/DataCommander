using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    internal sealed class ViewCollectionNode : ITreeNode
    {
        public ViewCollectionNode(DatabaseNode database)
        {
            this.database = database;
        }

        public string Name => "Views";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var treeNodes = new List<ITreeNode>();
            treeNodes.Add(new SystemViewCollectionNode(this.database));

            var databaseName = new SqlCommandBuilder().QuoteIdentifier(this.database.Name);
            var commandText = $@"select
    s.name,
    v.name,
    v.object_id
from {databaseName}.sys.schemas s (nolock)
join {databaseName}.sys.views v (nolock)
    on s.schema_id = v.schema_id
order by 1,2";
            using (var connection = new SqlConnection(this.database.Databases.Server.ConnectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        var schema = dataRecord.GetString(0);
                        var name = dataRecord.GetString(1);
                        var id = dataRecord.GetInt32(2);
                        treeNodes.Add(new ViewNode(this.database, id, schema, name));
                    });
                }
            }
            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        private readonly DatabaseNode database;
    }
}