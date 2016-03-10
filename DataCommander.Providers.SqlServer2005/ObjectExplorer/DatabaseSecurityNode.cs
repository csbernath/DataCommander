namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;

    internal sealed class DatabaseSecurityNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public DatabaseSecurityNode(DatabaseNode databaseNode)
        {
            Contract.Requires<ArgumentNullException>(databaseNode != null);
            this.databaseNode = databaseNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Security";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new UserCollectionNode(this.databaseNode),
                new RoleCollectionNode(this.databaseNode),
                new SchemaCollectionNode(this.databaseNode)
            };
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}