namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Linq;

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

        public LinkedServerCollectionNode LinkedServers
        {
            get
            {
                return this.linkedServers;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return this.name;
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
            return new LinkedServerCatalogCollectionNode( this ).ItemAsEnumerable();
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