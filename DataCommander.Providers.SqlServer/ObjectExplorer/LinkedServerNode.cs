using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class LinkedServerNode : ITreeNode
    {
        public LinkedServerNode(
            LinkedServerCollectionNode linkedServers,
            string name)
        {
            Assert.IsNotNull(linkedServers);
            LinkedServers = linkedServers;
            Name = name;
        }

        public LinkedServerCollectionNode LinkedServers { get; }

        public string Name { get; }

        #region ITreeNode Members

        string ITreeNode.Name => Name;

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new LinkedServerCatalogCollectionNode(this).ItemAsEnumerable();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}