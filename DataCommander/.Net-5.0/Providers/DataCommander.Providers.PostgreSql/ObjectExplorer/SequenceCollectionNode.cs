using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;
using Npgsql;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class SequenceCollectionNode : ITreeNode
    {
        private readonly SchemaNode _schemaNode;

        public SequenceCollectionNode(SchemaNode schemaNode)
        {
            _schemaNode = schemaNode;
        }

        string ITreeNode.Name => "Sequences";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            using (var connection = new NpgsqlConnection(_schemaNode.SchemaCollectionNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                var commandText = $@"select sequence_name
from information_schema.sequences
where sequence_schema = '{_schemaNode.Name}'
order by sequence_name";
                return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataReader =>
                {
                    var name = dataReader.GetString(0);
                    return new SequenceNode(this, name);
                });
            }
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}