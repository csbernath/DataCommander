namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Linq;

    internal sealed class ServerObjectCollectionNode : ITreeNode
    {
        private readonly ServerNode server;

        public ServerObjectCollectionNode(ServerNode serverNode)
        {
            Contract.Requires( serverNode != null );
            this.server = serverNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Server Objects";
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            return new LinkedServerCollectionNode( this.server ).ItemToArray(); 
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return null;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}