namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class TableNode : ITreeNode
    {
        private readonly TableCollectionNode tableCollectionNode;
        private readonly string name;

        public TableNode(TableCollectionNode tableCollectionNode, string name)
        {
            this.tableCollectionNode = tableCollectionNode;
            this.name = name;
        }

        public TableCollectionNode TableCollectionNode => this.tableCollectionNode;

        public string Name => this.name;

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

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}