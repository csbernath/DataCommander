using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
    public sealed class SequenceNode : ITreeNode
	{
		private SchemaNode schemaNode;
		private readonly string name;

		public SequenceNode( SchemaNode schemaNode, string name )
		{
			this.schemaNode = schemaNode;
			this.name = name;
		}

		#region ITreeNode Members

		string ITreeNode.Name => name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
		{
			throw new NotImplementedException();
		}

		bool ITreeNode.Sortable => throw new NotImplementedException();

	    string ITreeNode.Query => throw new NotImplementedException();

	    ContextMenuStrip ITreeNode.ContextMenu => throw new NotImplementedException();

	    #endregion
	}
}
