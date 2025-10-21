using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ProcedureNode(SchemaNode schemaNode, string? name, string[]? argnames) : ITreeNode
{
    public string? Name
    {
        get
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(name);

            if (argnames != null)
            {
                stringBuilder.Append('(');
                var args = string.Join(",", argnames!);
                stringBuilder.Append(args);
                stringBuilder.Append(')');
            }

            return stringBuilder.ToString();
        }
    }

    bool ITreeNode.IsLeaf => true;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    bool ITreeNode.Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}