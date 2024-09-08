using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ViewCollectionNode : ITreeNode
{
    private readonly SchemaNode _schemaNode;

    public ViewCollectionNode(SchemaNode schemaNode)
    {
        _schemaNode = schemaNode;
    }

    string? ITreeNode.Name => "Views";

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        using (var connection = _schemaNode.SchemaCollectionNode.ObjectExplorer.CreateConnection())
        {
            connection.Open();
            var executor = connection.CreateCommandExecutor();
            return Task.FromResult<IEnumerable<ITreeNode>>(executor.ExecuteReader(new ExecuteReaderRequest($@"select table_name
from information_schema.views
where table_schema = '{_schemaNode.Name}'
order by table_name"), 128, dataReader =>
            {
                var name = dataReader.GetString(0);
                return new ViewNode(this, name);
            }));
        }
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;
}