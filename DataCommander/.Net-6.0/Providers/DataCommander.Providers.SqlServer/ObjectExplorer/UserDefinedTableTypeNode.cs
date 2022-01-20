using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class UserDefinedTableTypeNode : ITreeNode
{
    private readonly DatabaseNode _database;
    private readonly int _id;
    private readonly string _name;
    private readonly string _schema;

    public UserDefinedTableTypeNode(DatabaseNode database, int id, string schema, string name)
    {
        _database = database;
        _id = id;
        _schema = schema;
        _name = name;
    }

    string ITreeNode.Name => $"{_schema}.{_name}";
    bool ITreeNode.IsLeaf => false;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
    {
        return new ITreeNode[]
        {
            new ColumnCollectionNode(_database, _id)
        };
    }

    bool ITreeNode.Sortable => false;
    string ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;
}