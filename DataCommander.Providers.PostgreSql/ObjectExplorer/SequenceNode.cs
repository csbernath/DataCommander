namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class SequenceNode : ITreeNode
    {
        private readonly SequenceCollectionNode sequenceCollectionNode;
        private readonly string name;

        public SequenceNode(SequenceCollectionNode sequenceCollectionNode, string name)
        {
            this.sequenceCollectionNode = sequenceCollectionNode;
            this.name = name;
        }

        string ITreeNode.Name => name;

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