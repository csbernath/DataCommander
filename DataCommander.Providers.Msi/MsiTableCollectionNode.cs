namespace DataCommander.Providers.Msi
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;

    internal sealed class MsiTableCollectionNode : ITreeNode
	{
		private MsiConnection connection;

		public MsiTableCollectionNode( MsiConnection connection )
		{
			this.connection = connection;
		}

		#region ITreeNode Members

		string ITreeNode.Name
		{
			get
			{
				return "Tables";
			}
		}

		bool ITreeNode.IsLeaf
		{
			get
			{
				return false;
			}
		}

		IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
		{
			var e = from table in this.connection.Database.Tables
					select (ITreeNode)new MsiTableNode( this.connection, table );

			return e;
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
