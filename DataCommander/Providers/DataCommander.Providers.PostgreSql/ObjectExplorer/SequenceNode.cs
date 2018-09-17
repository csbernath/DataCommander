using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class SequenceNode : ITreeNode
    {
        private readonly SequenceCollectionNode _sequenceCollectionNode;
        private readonly string _name;

        public SequenceNode(SequenceCollectionNode sequenceCollectionNode, string name)
        {
            this._sequenceCollectionNode = sequenceCollectionNode;
            this._name = name;
        }

        string ITreeNode.Name => _name;

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