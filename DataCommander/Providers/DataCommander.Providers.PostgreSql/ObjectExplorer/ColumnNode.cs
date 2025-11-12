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
    PostgresSqlType type,
    bool notNull) : ITreeNode
{
    string? ITreeNode.Name
    {
        get
        {
            var typeRepository = columnCollectionNode.TableNode.SchemaNode.SchemaCollectionNode.DatabaseNode.TypeRepository!;
            var typeName = typeRepository.TryGetPostgresSqlTypeName(type.Oid, out var postgresSqlTypeName)
                ? postgresSqlTypeName!.Name
                : type.Name;
            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"{name} ({typeName}");

            if (notNull)
                stringBuilder.Append(", not null");

            stringBuilder.Append(')');
            return stringBuilder.ToString();
        }
    }

    bool ITreeNode.IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>([]);

    bool ITreeNode.Sortable => false;

    public bool DynamicChildCount => false;

    Task<string?> ITreeNode.GetQuery(CancellationToken cancellationToken) => Task.FromResult<string?>(null);
    public ContextMenu? GetContextMenu() => null;
}