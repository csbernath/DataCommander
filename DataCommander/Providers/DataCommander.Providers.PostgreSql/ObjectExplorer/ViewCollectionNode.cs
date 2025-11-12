using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ViewCollectionNode(SchemaNode schemaNode) : ITreeNode
{
    string? ITreeNode.Name => "Views";

    bool ITreeNode.IsLeaf => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        using var connection = schemaNode.SchemaCollectionNode.DatabaseNode.CreateConnection();
        connection.Open();
        var executor = connection.CreateCommandExecutor();
        return Task.FromResult<IEnumerable<ITreeNode>>(executor.ExecuteReader(new ExecuteReaderRequest($@"select table_name
from information_schema.views
where table_schema = '{schemaNode.Name}'
order by table_name"), 128, dataReader =>
        {
            var name = dataReader.GetString(0);
            return new ViewNode(name);
        }));
    }

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => true;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}