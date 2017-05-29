namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class ViewNode : ITreeNode
    {
        private readonly ViewCollectionNode viewCollectionNode;
        private readonly string name;

        public ViewNode(ViewCollectionNode viewCollectionNode, string name)
        {
            this.viewCollectionNode = viewCollectionNode;
            this.name = name;
        }

        string ITreeNode.Name => this.name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => null;

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}