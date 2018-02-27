namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class ColumnNode : ITreeNode
    {
        private readonly ColumnCollectionNode columnCollectionNode;
        private readonly string name;
        private readonly string dataType;

        public ColumnNode(ColumnCollectionNode columnCollectionNode, string name, string dataType)
        {
            this.columnCollectionNode = columnCollectionNode;
            this.name = name;
            this.dataType = dataType;
        }

        string ITreeNode.Name => $"{name} {dataType}";

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}