using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Windows.Forms;

    internal sealed class SchemaCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _database;

        public SchemaCollectionNode(DatabaseNode  database)
        {
            _database = database;
        }

        public string Name => "Schemas";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = @"select s.name
from {0}.sys.schemas s (nolock)
order by s.name";

            var sqlCommandBuilder = new SqlCommandBuilder();
            commandText = string.Format(commandText, sqlCommandBuilder.QuoteIdentifier(_database.Name));
            var connectionString = _database.Databases.Server.ConnectionString;
            var treeNodes = new List<ITreeNode>();

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);
                using (var dataReader = transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default))
                {
                    dataReader.Read(dataRecord =>
                    {
                        var name = dataRecord.GetString(0);
                        var treeNode = new SchemaNode(_database, name);
                        treeNodes.Add(treeNode);
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