using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class TableCollectionNode : ITreeNode
    {
        public TableCollectionNode(SchemaNode schemaNode)
        {
            SchemaNode = schemaNode;
        }

        public SchemaNode SchemaNode { get; }

        string ITreeNode.Name => "Tables";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();
            using (var connection = new NpgsqlConnection(SchemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                var commandText = $@"select table_name
from information_schema.tables
where
    table_schema = '{SchemaNode.Name}'
    and table_type = 'BASE TABLE'
order by table_name";
                executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
                {
                    dataReader.ReadResult(() =>
                    {
                        var name = dataReader.GetString(0);
                        var schemaNode = new TableNode(this, name);
                        nodes.Add(schemaNode);
                        return true;

                    });
                });
            }

            return nodes;
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}