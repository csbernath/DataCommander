namespace DataCommander.Providers
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    public interface ITreeNode
    {
        string Name { get; }

        bool IsLeaf { get; }

        IEnumerable<ITreeNode> GetChildren(bool refresh);

        bool Sortable { get; }

        string Query { get; }

        ContextMenuStrip ContextMenu { get; }
    }
}