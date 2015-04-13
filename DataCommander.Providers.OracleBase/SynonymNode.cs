namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Data;
    using DataCommander.Foundation.Data;
    using DataCommander.Providers;

	public sealed class SynonymNode : ITreeNode
	{
		private SchemaNode schema;
		private string name;

		public SynonymNode( SchemaNode schema, string name )
		{
			this.schema = schema;
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
				return false;
			}
		}

		IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
		{
			string commandText = string.Format( @"
select	s.TABLE_OWNER,
	s.TABLE_NAME
from	SYS.ALL_SYNONYMS s
where	s.OWNER			= '{0}'
	and s.SYNONYM_NAME	= '{1}'", this.schema.Name, this.name );

			DataTable dataTable = this.schema.SchemasNode.Connection.ExecuteDataTable( commandText );
			DataRow dataRow = dataTable.Rows[ 0 ];
			string schemaName = (string) dataRow[ "TABLE_OWNER" ];
			SchemaNode schemaNode = new SchemaNode( this.schema.SchemasNode, schemaName );
			TableNode tableNode = new TableNode( schemaNode, (string) dataRow[ "TABLE_NAME" ], true );
			return new ITreeNode[] { tableNode };
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
