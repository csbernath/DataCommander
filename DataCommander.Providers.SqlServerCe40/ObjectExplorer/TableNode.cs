namespace DataCommander.Providers.SqlServerCe40.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class TableNode : ITreeNode
    {
        readonly string name;

        public TableNode( string name )
        {
            this.name = name;
        }

        #region ITreeNode Members

        string ITreeNode.Name => name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}