namespace DataCommander.Providers.Odp
{
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;

    internal sealed class FunctionCollectionNode : ITreeNode
	{
		private SchemaNode schemaNode;

		public FunctionCollectionNode( SchemaNode schemaNode )
		{
			this.schemaNode = schemaNode;
		}

		#region ITreeNode Members

		string ITreeNode.Name
		{
			get
			{
				return "Functions";
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
			string commandText = string.Format( @"select	OBJECT_NAME
from	SYS.ALL_OBJECTS
where	OWNER	= '{0}'
	and OBJECT_TYPE	= 'FUNCTION'
order by OBJECT_NAME", schemaNode.Name );
			List<ITreeNode> list = new List<ITreeNode>();

			using (IDataReader dataReader = schemaNode.SchemasNode.Connection.ExecuteReader( commandText ))
			{
				while (dataReader.Read())
				{
					string procedureName = dataReader.GetString( 0 );
					FunctionNode functionNode = new FunctionNode( schemaNode, null, procedureName );
					list.Add( functionNode );
				}
			}

			return list.ToArray();
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