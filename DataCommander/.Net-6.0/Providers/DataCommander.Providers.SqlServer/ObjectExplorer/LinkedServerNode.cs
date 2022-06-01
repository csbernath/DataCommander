using System;
using System.Collections.Generic;
using DataCommander.Api;
using Foundation.Assertions;
using Foundation.Linq;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LinkedServerNode : ITreeNode
{
    public LinkedServerNode(LinkedServerCollectionNode linkedServers, string name)
    {
        ArgumentNullException.ThrowIfNull(linkedServers);
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

    public ContextMenu? GetContextMenu()
    {
        throw new System.NotImplementedException();
    }
}