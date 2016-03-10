namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;

    internal sealed class LinkedServerCatalogNode : ITreeNode
    {
        private readonly string name;

        public LinkedServerCatalogNode(LinkedServerNode linkedServer, string name)
        {
            Contract.Requires(linkedServer != null);
            this.name = name;
        }

        #region ITreeNode Members

        string ITreeNode.Name => this.name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}
