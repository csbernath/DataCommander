using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
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

        public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
        {
            using (var connection = _schemaNode.SchemaCollectionNode.ObjectExplorer.CreateConnection())
            {
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                var commandText = $@"select sequence_name
from information_schema.sequences
where sequence_schema = '{_schemaNode.Name}'
order by sequence_name";
                return Task.FromResult<IEnumerable<ITreeNode>>(executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataReader =>
                {
                    var name = dataReader.GetString(0);
                    return new SequenceNode(this, name);
                }));
            }
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        public ContextMenu? GetContextMenu() => null;
    }
}