using System.Collections.Generic;
using System.Threading;
using DataCommander.Api;
using Foundation.Data;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp.ObjectExplorer;

/// <summary>
/// Summary description for TablesNode.
/// </summary>
internal sealed class TableCollectionNode : ITreeNode
{
	public TableCollectionNode( SchemaNode schema )
	{
		_schema = schema;
	}

	public string Name => "Tables";

	public bool IsLeaf => false;

	public IEnumerable<ITreeNode> GetChildren(bool refresh)
	{
		// var folder = DataCommanderApplication.Instance.ApplicationData.CurrentType;
		// var key = _schema.SchemasNode.Connection.DataSource + "." + _schema.Name;
		// var contains = folder.Attributes.TryGetAttributeValue( key, out string[] tables );
		string[] tables;

		//if (!contains || refresh)
		if (true)
		{
			var commandText = "select table_name from all_tables where owner = '{0}' order by table_name";
			commandText = string.Format(commandText, _schema.Name);

			var command = new OracleCommand(commandText, _schema.SchemasNode.Connection);
			command.FetchSize = 256 * 1024;
			var dataTable = command.ExecuteDataTable(CancellationToken.None);
			var count = dataTable.Rows.Count;
			tables = new string[count];

			for (var i = 0; i < count; i++)
			{
				var name = (string)dataTable.Rows[i][0];
				tables[i] = name;
			}

			//folder.Attributes.SetAttributeValue( key, tables );
		}

		var treeNodes = new ITreeNode[tables.Length];

		for (var i = 0; i < tables.Length; i++)
			treeNodes[i] = new TableNode(_schema, tables[i], false);

		return treeNodes;
	}

	public bool Sortable => false;
	public string Query => null;

	public ContextMenu GetContextMenu()
	{
		throw new System.NotImplementedException();
	}

	private readonly SchemaNode _schema;
}