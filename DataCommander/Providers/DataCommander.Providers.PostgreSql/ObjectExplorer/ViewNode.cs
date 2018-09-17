using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class ViewNode : ITreeNode
    {
        private readonly ViewCollectionNode _viewCollectionNode;
        private readonly string _name;

        public ViewNode(ViewCollectionNode viewCollectionNode, string name)
        {
            this._viewCollectionNode = viewCollectionNode;
            this._name = name;
        }

        string ITreeNode.Name => _name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => null;

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}