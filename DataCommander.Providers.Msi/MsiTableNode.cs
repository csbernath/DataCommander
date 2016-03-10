namespace DataCommander.Providers.Msi
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Microsoft.Deployment.WindowsInstaller;

    internal sealed class MsiTableNode : ITreeNode
	{
		private MsiConnection connection;
		private TableInfo table;

		public MsiTableNode( MsiConnection connection, TableInfo table )
		{
			this.connection = connection;
			this.table = table;
		}

		#region ITreeNode Members

		string ITreeNode.Name => this.table.Name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
		{
			return null;
		}

		bool ITreeNode.Sortable => false;

        string ITreeNode.Query => this.table.SqlSelectString;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
	}
}
