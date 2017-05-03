namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class LinkedServerCatalogNode : ITreeNode
    {
        private readonly string name;

        public LinkedServerCatalogNode(LinkedServerNode linkedServer, string name)
        {
#if CONTRACTS_FULL
            Contract.Requires(linkedServer != null);
#endif
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
