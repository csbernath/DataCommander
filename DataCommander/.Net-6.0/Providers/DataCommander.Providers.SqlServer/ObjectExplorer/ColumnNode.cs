using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ColumnNode : ITreeNode
{
    private readonly string _columnName;
    private readonly bool _isNullable;
    private readonly short _maxLength;
    private readonly byte _precision;
    private readonly byte _scale;
    private readonly byte _systemTypeId;
    private readonly string _userTypeName;
    private bool _isForeignKey;
    private bool _isPrimaryKey;

    public ColumnNode(int id, string columnName, byte systemTypeId, short maxLength, byte precision, byte scale, bool isNullable, string userTypeName)
    {
        Id = id;
        _columnName = columnName;
        _systemTypeId = systemTypeId;
        _maxLength = maxLength;
        _precision = precision;
        _scale = scale;
        _isNullable = isNullable;
        _userTypeName = userTypeName;
    }

    public int Id { get; }

    public bool IsPrimaryKey
    {
        set => _isPrimaryKey = value;
    }

    public bool IsForeignKey
    {
        set => _isForeignKey = value;
    }

    #region ITreeNode Members

    string ITreeNode.Name
    {
        get
        {
            string typeName;
            var systemType = (SqlServerSystemType) _systemTypeId;
            switch (systemType)
            {
                case SqlServerSystemType.Char:
                case SqlServerSystemType.VarBinary:
                case SqlServerSystemType.VarChar:
                    var maxLengthString = _maxLength >= 0 ? _maxLength.ToString() : "max";
                    typeName = $"{_userTypeName}({maxLengthString})";
                    break;

                case SqlServerSystemType.NChar:
                case SqlServerSystemType.NVarChar:
                    maxLengthString = _maxLength >= 0 ? (_maxLength / 2).ToString() : "max";
                    typeName = $"{_userTypeName}({maxLengthString})";
                    break;

                case SqlServerSystemType.Decimal:
                case SqlServerSystemType.Numeric:
                    if (_scale == 0)
                        typeName = $"{_userTypeName}({_precision})";
                    else
                        typeName = $"{_userTypeName}({_precision},{_scale})";
                    break;

                default:
                    typeName = _userTypeName;
                    break;
            }

            return
                $"{_columnName} ({(_isPrimaryKey ? "PK, " : null)}{(_isForeignKey ? "FK, " : null)}{typeName}, {(_isNullable ? "null" : "not null")})";
        }
    }

    bool ITreeNode.IsLeaf => true;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return null;
    }

    bool ITreeNode.Sortable => false;

    string ITreeNode.Query => null;

    public ContextMenu GetContextMenu() => null;

    #endregion
}