using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class SchemaCollectionNode(ObjectExplorer objectExplorer) : ITreeNode
{
    public ObjectExplorer ObjectExplorer { get; } = objectExplorer;
    bool ITreeNode.IsLeaf => false;
    string? ITreeNode.Name => "Schemas";
    string ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;

    bool ITreeNode.Sortable => false;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        using (var connection = ObjectExplorer.CreateConnection())
        {
            connection.Open();
            var executor = connection.CreateCommandExecutor();
            return Task.FromResult<IEnumerable<ITreeNode>>(executor.ExecuteReader(new ExecuteReaderRequest(@"select schema_name
from information_schema.schemata
order by schema_name"), 128, dataReader =>
            {
                var name = dataReader.GetString(0);
                return new SchemaNode(this, name);
            }));
        }
    }
}