﻿using Foundation.Assertions;
using Foundation.Linq;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class LinkedServerNode : ITreeNode
    {
        public LinkedServerNode(LinkedServerCollectionNode linkedServers, string name)
        {
            Assert.IsNotNull(linkedServers);
            LinkedServers = linkedServers;
            Name = name;
        }

        public LinkedServerCollectionNode LinkedServers { get; }
        public string Name { get; }
        string ITreeNode.Name => Name;
        bool ITreeNode.IsLeaf => false;
        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => new LinkedServerCatalogCollectionNode(this).ItemAsEnumerable();
        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}