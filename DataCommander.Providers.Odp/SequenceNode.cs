namespace DataCommander.Providers.Odp
{
	using System;
	using System.Collections.Generic;
    using DataCommander.Providers;

	internal sealed class SequenceNode : ITreeNode
	{
		private SchemaNode schemaNode;
		private string name;

		public SequenceNode( SchemaNode schemaNode, string name )
		{
			this.schemaNode = schemaNode;
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
				throw new NotImplementedException();
			}
		}

		string ITreeNode.Query
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
