using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class SchemaCollectionNode : ITreeNode
    {
        private readonly DatabaseNode _database;

        public SchemaCollectionNode(DatabaseNode database) => _database = database;

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
                var executor = connection.CreateCommandExecutor();
                return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new SchemaNode(_database, name);
                });
            }
        }

        public bool Sortable => false;
        public string Query => null;
        public ContextMenuStrip ContextMenu => null;
    }
}