using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class TableNode : ITreeNode
    {
        public TableNode(TableCollectionNode tableCollectionNode, string name)
        {
            TableCollectionNode = tableCollectionNode;
            Name = name;
        }

        public TableCollectionNode TableCollectionNode { get; }

        public string Name { get; }

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new ColumnCollectionNode(this)
            };
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        public ContextMenu? GetContextMenu() => null;
    }
}