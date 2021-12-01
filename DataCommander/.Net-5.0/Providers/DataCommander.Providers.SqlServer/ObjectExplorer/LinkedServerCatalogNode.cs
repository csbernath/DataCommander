using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Assertions;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class LinkedServerCatalogNode : ITreeNode
    {
        private readonly string _name;

        public LinkedServerCatalogNode(LinkedServerNode linkedServer, string name)
        {
            Assert.IsNotNull(linkedServer);
            _name = name;
        }

        #region ITreeNode Members

        string ITreeNode.Name => _name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;
        public ContextMenu GetContextMenu()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}