namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class SystemViewCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public SystemViewCollectionNode(DatabaseNode databaseNode)
        {
            this.databaseNode = databaseNode;
        }

        public string Name => "System Views";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var cb = new SqlCommandBuilder();
            var databaseName = cb.QuoteIdentifier(this.databaseNode.Name);
            var commandText = $@"select
    s.name,
    v.name,
    v.object_id
from {databaseName}.sys.schemas s (nolock)
join {databaseName}.sys.system_views v (nolock)
    on s.schema_id = v.schema_id
order by 1,2";
            commandText = string.Format(commandText, this.databaseNode.Name);
            var treeNodes = new List<ViewNode>();
            var connectionString = this.databaseNode.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
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
                        var viewNode = new ViewNode(databaseNode, id, schema, name);
                        treeNodes.Add(viewNode);
                    });
                }
            }
            return treeNodes;
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;
    }
}