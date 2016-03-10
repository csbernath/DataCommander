namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using Foundation.Linq;

    internal sealed class ServerObjectCollectionNode : ITreeNode
    {
        private readonly ServerNode server;

        public ServerObjectCollectionNode(ServerNode serverNode)
        {
            Contract.Requires( serverNode != null );
            this.server = serverNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Server Objects";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return new LinkedServerCollectionNode( this.server ).ItemToArray(); 
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}