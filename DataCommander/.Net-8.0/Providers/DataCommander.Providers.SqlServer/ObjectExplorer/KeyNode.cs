using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal class KeyNode : ITreeNode
{
    private readonly string _name;

    public KeyNode(DatabaseNode databaseNode, int id, string name)
    {
        _name = name;
    }

    public string Name => _name;
    public bool IsLeaf => true;
    public IEnumerable<ITreeNode> GetChildren(bool refresh) => null;
    public bool Sortable => false;
    public string Query => null;
    public ContextMenu? GetContextMenu() => null;
}