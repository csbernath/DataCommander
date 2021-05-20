using Foundation.Assertions;
using Foundation.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class ServerObjectCollectionNode : ITreeNode
    {
        private readonly ServerNode _server;

        public ServerObjectCollectionNode(ServerNode serverNode)
        {
            Assert.IsNotNull(serverNode);
            _server = serverNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Server Objects";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new LinkedServerCollectionNode(_server).ItemToArray();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}