namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using Foundation.Linq;

    internal sealed class LinkedServerNode : ITreeNode
    {
        private readonly LinkedServerCollectionNode linkedServers;
        private readonly string name;

        public LinkedServerNode(
            LinkedServerCollectionNode linkedServers,
            string name )
        {
            Contract.Requires( linkedServers != null );
            this.linkedServers = linkedServers;
            this.name = name;
        }

        public LinkedServerCollectionNode LinkedServers => this.linkedServers;

        public string Name => this.name;

        #region ITreeNode Members

        string ITreeNode.Name => this.name;

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return new LinkedServerCatalogCollectionNode( this ).ItemAsEnumerable();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}