namespace DataCommander.Providers.Msi
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using DataCommander.Providers;
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

		string ITreeNode.Name
		{
			get
			{
				return this.table.Name;
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
			return null;
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
				return this.table.SqlSelectString;
			}
		}

		System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
		{
			get
			{
				return null;
			}
		}

		#endregion
	}
}
