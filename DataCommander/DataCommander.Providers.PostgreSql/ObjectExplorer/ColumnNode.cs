using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.PostgreSql.ObjectExplorer
{
    internal sealed class ColumnNode : ITreeNode
    {
        private readonly ColumnCollectionNode _columnCollectionNode;
        private readonly string _name;
        private readonly string _dataType;

        public ColumnNode(ColumnCollectionNode columnCollectionNode, string name, string dataType)
        {
            this._columnCollectionNode = columnCollectionNode;
            this._name = name;
            this._dataType = dataType;
        }

        string ITreeNode.Name => $"{_name} {_dataType}";

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