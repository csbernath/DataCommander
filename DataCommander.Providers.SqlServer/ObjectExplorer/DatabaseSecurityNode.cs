using System;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class DatabaseSecurityNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;

        public DatabaseSecurityNode(DatabaseNode databaseNode)
        {
            FoundationContract.Requires<ArgumentNullException>(databaseNode != null);
            _databaseNode = databaseNode;
        }

#region ITreeNode Members

        string ITreeNode.Name => "Security";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new UserCollectionNode(_databaseNode),
                new RoleCollectionNode(_databaseNode),
                new SchemaCollectionNode(_databaseNode)
            };
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

#endregion
    }
}