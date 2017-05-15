namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Windows.Forms;
    using Foundation.Data;

    internal sealed class TriggerCollectionNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;
        private readonly int id;

        public TriggerCollectionNode(DatabaseNode databaseNode, int id)
        {
            this.databaseNode = databaseNode;
            this.id = id;
        }

        public string Name => "Triggers";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var cb = new SqlCommandBuilder();
            var databaseName = cb.QuoteIdentifier(this.databaseNode.Name);

            var commandText = $@"select
    name,
    object_id
from {databaseName}.sys.triggers
where object_id = {this.id}
order name";

            var connectionString = this.databaseNode.Databases.Server.ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    return dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var id = dataRecord.GetInt32(1);
                        return new TriggerNode(this.databaseNode, id, name);
                    }).ToList();
                }
            }
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;
    }
}