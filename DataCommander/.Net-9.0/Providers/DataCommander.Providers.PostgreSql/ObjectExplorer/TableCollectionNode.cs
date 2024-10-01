using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class TableCollectionNode(SchemaNode schemaNode) : ITreeNode
{
    public SchemaNode SchemaNode { get; } = schemaNode;

    string? ITreeNode.Name => "Tables";

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        using var connection = SchemaNode.SchemaCollectionNode.ObjectExplorer.CreateConnection();
        connection.Open();
        var executor = connection.CreateCommandExecutor();
        var commandText = $@"select table_name
from information_schema.tables
where
    table_schema = '{SchemaNode.Name}'
    and table_type = 'BASE TABLE'
order by table_name";
        return Task.FromResult<IEnumerable<ITreeNode>>(executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataReader =>
        {
            var name = dataReader.GetString(0);
            return new TableNode(this, name);
        }));
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}