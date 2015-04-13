namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Linq;

    internal sealed class DatabaseSecurityNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;

        public DatabaseSecurityNode(DatabaseNode databaseNode)
        {
            Contract.Requires<ArgumentNullException>(databaseNode != null);
            this.databaseNode = databaseNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Security";
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new UserCollectionNode(this.databaseNode),
                new RoleCollectionNode(this.databaseNode)
            };
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