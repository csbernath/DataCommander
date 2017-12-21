using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class ServerObjectCollectionNode : ITreeNode
    {
        private readonly ServerNode _server;

        public ServerObjectCollectionNode(ServerNode serverNode)
        {
#if CONTRACTS_FULL
            FoundationContract.Requires( serverNode != null );
#endif
            _server = serverNode;
        }

#region ITreeNode Members

        string ITreeNode.Name => "Server Objects";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return new LinkedServerCollectionNode( _server ).ItemToArray(); 
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}