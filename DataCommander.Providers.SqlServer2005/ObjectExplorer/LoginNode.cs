namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class LoginNode : ITreeNode
    {
        private readonly string name;

        public LoginNode( string name )
        {
            this.name = name;
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
                return true;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            throw new NotImplementedException();
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