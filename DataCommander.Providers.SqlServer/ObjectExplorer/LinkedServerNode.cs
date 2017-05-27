using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class LinkedServerNode : ITreeNode
    {
        public LinkedServerNode(
            LinkedServerCollectionNode linkedServers,
            string name )
        {
#if CONTRACTS_FULL
            Contract.Requires( linkedServers != null );
#endif
            LinkedServers = linkedServers;
            Name = name;
        }

        public LinkedServerCollectionNode LinkedServers { get; }

        public string Name { get; }

#region ITreeNode Members

        string ITreeNode.Name => Name;

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