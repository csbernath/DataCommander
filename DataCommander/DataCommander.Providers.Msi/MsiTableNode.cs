namespace DataCommander.Providers.Msi
{
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Microsoft.Deployment.WindowsInstaller;

    internal sealed class MsiTableNode : ITreeNode
	{
		private MsiConnection _connection;
		private readonly TableInfo _table;

		public MsiTableNode( MsiConnection connection, TableInfo table )
		{
			_connection = connection;
			_table = table;
		}

		#region ITreeNode Members

		string ITreeNode.Name => _table.Name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
		{
			return null;
		}

		bool ITreeNode.Sortable => false;

        string ITreeNode.Query => _table.SqlSelectString;

        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
	}
}
