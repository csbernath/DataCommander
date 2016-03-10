namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class SequenceNode : ITreeNode
	{
		private SchemaNode schemaNode;
		private readonly string name;

		public SequenceNode( SchemaNode schemaNode, string name )
		{
			this.schemaNode = schemaNode;
			this.name = name;
		}

		#region ITreeNode Members

		string ITreeNode.Name => this.name;

        bool ITreeNode.IsLeaf => true;

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

		ContextMenuStrip ITreeNode.ContextMenu
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
