using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer;

internal sealed class ColumnNode(
    ColumnCollectionNode columnCollectionNode,
    string name,
    uint typeOid,
    bool notNull) : ITreeNode
{
    string? ITreeNode.Name
    {
        get
        {
            if (!PostgresSqlTypeRepository.TryGetByOid(typeOid, out var type))
            {
                var types = columnCollectionNode.TableNode.SchemaNode.SchemaCollectionNode.DatabaseNode.PostgresSqlTypes;
                types.TryGetValue(typeOid, out type);
            }

            var dataTypeName = type != null
                ? type.Name
                : typeOid.ToString();
            
            var sb = new StringBuilder();
            sb.Append($"{name} ({dataTypeName}");

            if (notNull)
                sb.Append(", not null");

            sb.Append(')');
            return sb.ToString();
        }
    }

    bool ITreeNode.IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}