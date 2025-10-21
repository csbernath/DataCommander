using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ProcedureNode(SchemaNode schemaNode, string? name, char[]? argmodes, string[]? argnames, uint[]? allargtypes) : ITreeNode
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
                for (var index = 0; index < argnames.Length; ++index)
                {
                    if (index > 0)
                        stringBuilder.Append(", ");

                    var mode = GetMode(index);

                    stringBuilder.Append(mode);
                    stringBuilder.Append(' ');
                    stringBuilder.Append(argnames[index]);
                    stringBuilder.Append(' ');

                    var argtypeOid = allargtypes[index];
                    if (PostgresSqlTypeRepository.TryGetByOid(argtypeOid, out var postgresSqlType))
                        stringBuilder.Append(postgresSqlType!.Name);
                    else
                        stringBuilder.Append(argtypeOid);
                }

                stringBuilder.Append(')');
            }

            return stringBuilder.ToString();
        }
    }

    private string? GetMode(int index)
    {
        var mode = argmodes![index] switch
        {
            'i' => "IN",
            'b' => "INOUT",
            'o' => "OUT",
            _ => null
        };
        return mode;
    }

    bool ITreeNode.IsLeaf => true;

    public Task<IEnumerable<ITreeNode>> GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    bool ITreeNode.Sortable => false;
    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}