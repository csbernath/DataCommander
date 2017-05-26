using Foundation.Data;

namespace DataCommander.Providers.OracleBase
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Windows.Forms;

    /// <summary>
	/// Summary description for TablesNode.
	/// </summary>
	public sealed class TableCollectionNode : ITreeNode
	{
		public TableCollectionNode( SchemaNode schema )
		{
			this.schema = schema;
		}

		public string Name => "Tables";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren( bool refresh )
		{
			var folder = DataCommanderApplication.Instance.ApplicationData.CurrentType;
			var key = schema.SchemasNode.Connection.Database + "." + schema.Name;
			string[] tables;
			var contains = folder.Attributes.TryGetAttributeValue( key, out tables );

			if (!contains || refresh)
			{
				var commandText = "select table_name from all_tables where owner = '{0}' order by table_name";
				commandText = string.Format( commandText, schema.Name );

                var command = schema.SchemasNode.Connection.CreateCommand();
                command.CommandText = commandText;
				// TODO
                // command.FetchSize = 256 * 1024;
			    var dataTable = command.ExecuteDataTable(CancellationToken.None);
				var count = dataTable.Rows.Count;
				tables = new string[ count ];

				for (var i = 0; i < count; i++)
				{
					var name = (string) dataTable.Rows[ i ][ 0 ];
					tables[ i ] = name;
				}

				folder.Attributes.SetAttributeValue( key, tables );
			}

			var treeNodes = new ITreeNode[ tables.Length ];

			for (var i = 0; i < tables.Length; i++)
			{
				treeNodes[ i ] = new TableNode( this.schema, tables[ i ], false );
			}

			return treeNodes;
		}

		public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        readonly SchemaNode schema;
	}
}