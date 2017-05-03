namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Foundation.Linq;

    internal sealed class ServerObjectCollectionNode : ITreeNode
    {
        private readonly ServerNode server;

        public ServerObjectCollectionNode(ServerNode serverNode)
        {
#if CONTRACTS_FULL
            Contract.Requires( serverNode != null );
#endif
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