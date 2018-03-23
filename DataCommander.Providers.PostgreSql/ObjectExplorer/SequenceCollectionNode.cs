using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class SequenceCollectionNode : ITreeNode
    {
        private readonly SchemaNode schemaNode;

        public SequenceCollectionNode(SchemaNode schemaNode)
        {
            this.schemaNode = schemaNode;
        }

        string ITreeNode.Name => "Sequences";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var nodes = new List<ITreeNode>();

            using (var connection = new NpgsqlConnection(this.schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                var commandText = $@"select sequence_name
from information_schema.sequences
where sequence_schema = '{this.schemaNode.Name}'
order by sequence_name";
                executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
                {
                    dataReader.ReadResult(() =>
                    {
                        var name = dataReader.GetString(0);
                        var schemaNode = new SequenceNode(this, name);
                        nodes.Add(schemaNode);
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