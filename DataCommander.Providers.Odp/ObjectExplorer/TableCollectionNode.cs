namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;
    using System.Windows.Forms;
    using Foundation.Configuration;
    using Foundation.Data;
    using Oracle.ManagedDataAccess.Client;

    /// <summary>
	/// Summary description for TablesNode.
	/// </summary>
	internal sealed class TableCollectionNode : ITreeNode
	{
		public TableCollectionNode( SchemaNode schema )
		{
			this.schema = schema;
		}

		public string Name => "Tables";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren( bool refresh )
		{
			ConfigurationNode folder = DataCommanderApplication.Instance.ApplicationData.CurrentType;
			string key = schema.SchemasNode.Connection.DataSource + "." + schema.Name;
			string[] tables;
			bool contains = folder.Attributes.TryGetAttributeValue( key, out tables );

			if (!contains || refresh)
			{
				string commandText = "select table_name from all_tables where owner = '{0}' order by table_name";
				commandText = string.Format( commandText, schema.Name );

				OracleCommand command = new OracleCommand( commandText, schema.SchemasNode.Connection );
				command.FetchSize = 256 * 1024;
				DataTable dataTable = command.ExecuteDataTable(CancellationToken.None);
				int count = dataTable.Rows.Count;
				tables = new string[ count ];

				for (int i = 0; i < count; i++)
				{
					string name = (string) dataTable.Rows[ i ][ 0 ];
					tables[ i ] = name;
				}

				folder.Attributes.SetAttributeValue( key, tables );
			}

			ITreeNode[] treeNodes = new ITreeNode[ tables.Length ];

			for (int i = 0; i < tables.Length; i++)
			{
				treeNodes[ i ] = new TableNode( this.schema, tables[ i ], false );
			}

			return treeNodes;
		}

		public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;

        public void BeforeExpand()
		{
		}

        readonly SchemaNode schema;
	}
}