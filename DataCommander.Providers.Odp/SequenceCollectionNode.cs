namespace DataCommander.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using DataCommander.Foundation.Data;
    using DataCommander.Providers;

	internal sealed class SequenceCollectionNode : ITreeNode
	{
		private SchemaNode schemaNode;

		public SequenceCollectionNode( SchemaNode schemaNode )
		{
			this.schemaNode = schemaNode;
		}

		#region ITreeNode Members

		string ITreeNode.Name
		{
			get
			{
				return "Sequences";
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
			string commandText = string.Format( @"select	s.SEQUENCE_NAME
from	SYS.ALL_SEQUENCES s
where	s.SEQUENCE_OWNER	= '{0}'
order by s.SEQUENCE_NAME
", this.schemaNode.Name );

			using (IDataReader dataReader = this.schemaNode.SchemasNode.Connection.ExecuteReader( commandText ))
			{
				while (dataReader.Read())
				{
					string name = dataReader.GetString( 0 );
					yield return new SequenceNode( this.schemaNode, name );
				}
			}
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