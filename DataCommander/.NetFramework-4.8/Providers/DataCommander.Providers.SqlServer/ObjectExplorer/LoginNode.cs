using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class LoginNode : ITreeNode
    {
        private readonly string _name;

        public LoginNode(string name) => _name = name;

        string ITreeNode.Name => _name;
        bool ITreeNode.IsLeaf => true;
        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => throw new NotImplementedException();
        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;
    }
}