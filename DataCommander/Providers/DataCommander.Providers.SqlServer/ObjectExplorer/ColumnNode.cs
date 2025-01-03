using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ColumnNode(
    int id,
    string columnName,
    byte systemTypeId,
    short maxLength,
    byte precision,
    byte scale,
    bool isNullable,
    bool isComputed,
    string userTypeName)
    : ITreeNode
{
    private bool _isForeignKey;
    private bool _isPrimaryKey;

    public int Id { get; } = id;

    public bool IsPrimaryKey
    {
        set => _isPrimaryKey = value;
    }

    public bool IsForeignKey
    {
        set => _isForeignKey = value;
    }

    string ITreeNode.Name
    {
        get
        {
            string typeName;
            var systemType = (SqlServerSystemType)systemTypeId;
            switch (systemType)
            {
                case SqlServerSystemType.Char:
                case SqlServerSystemType.VarBinary:
                case SqlServerSystemType.VarChar:
                    var maxLengthString = maxLength >= 0 ? maxLength.ToString() : "max";
                    typeName = $"{userTypeName}({maxLengthString})";
                    break;

                case SqlServerSystemType.NChar:
                case SqlServerSystemType.NVarChar:
                    maxLengthString = maxLength >= 0 ? (maxLength / 2).ToString() : "max";
                    typeName = $"{userTypeName}({maxLengthString})";
                    break;

                case SqlServerSystemType.Decimal:
                case SqlServerSystemType.Numeric:
                    if (scale == 0)
                        typeName = $"{userTypeName}({precision})";
                    else
                        typeName = $"{userTypeName}({precision},{scale})";
                    break;

                default:
                    typeName = userTypeName;
                    break;
            }

            return
                $"{columnName} ({(_isPrimaryKey ? "PK, " : null)}{(_isForeignKey ? "FK, " : null)}{(isComputed ? "Computed, " : null)}{typeName}, {(isNullable ? "null" : "not null")})";
        }
    }

    bool ITreeNode.IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) =>
        Task.FromResult<IEnumerable<ITreeNode>>(Array.Empty<ITreeNode>());

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;
}