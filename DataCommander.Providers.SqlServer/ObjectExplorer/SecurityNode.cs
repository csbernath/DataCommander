using System;
using Foundation.Diagnostics.Contracts;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class SecurityNode : ITreeNode
    {
        private readonly ServerNode _server;

        public SecurityNode(ServerNode serverNode)
        {
            FoundationContract.Requires<ArgumentException>( serverNode != null );

            _server = serverNode;
        }

#region ITreeNode Members

        string ITreeNode.Name => "Security";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return new LoginCollectionNode( _server ).ItemToArray();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}