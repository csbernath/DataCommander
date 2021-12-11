using System;
using System.Collections.Generic;
using DataCommander.Providers2;
using Foundation.Collections.ReadOnly;
using Oracle.ManagedDataAccess.Client;

namespace DataCommander.Providers.Odp.ObjectExplorer;

internal sealed class ViewNode : ITreeNode
{
	private readonly ViewCollectionNode _parent;
	private readonly string _name;

	public ViewNode( ViewCollectionNode parent, string name )
	{
		_parent = parent;
		_name = name;
	}

	public string Name => _name;

	public bool IsLeaf => true;

	public IEnumerable<ITreeNode> GetChildren( bool refresh )
	{
		return null;
	}

	public bool Sortable => false;

	public string Query
	{
		get
		{
			var query = $"select * from {_parent.SchemaNode.Name}.{_name}";
			return query;
		}
	}

	private void menuItemScriptObject_Click( object sender, EventArgs e )
	{
		var commandText = "select text from sys.all_views where owner = '{0}' and view_name = '{1}'";
		commandText = string.Format( commandText, _parent.SchemaNode.Name, _name );

		using (var command = new OracleCommand( commandText, _parent.SchemaNode.SchemasNode.Connection ))
		{
			command.InitialLONGFetchSize = 64 * 1024;

			using (var dataReader = command.ExecuteReader())
			{
				if (dataReader.Read())
				{
					var append = dataReader.GetString( 0 );

					var queryForm = (IQueryForm)sender;
					queryForm.ShowText(append);
				}

				dataReader.Close();
			}
		}
	}

	public ContextMenu GetContextMenu()
	{
		var menuItemScriptObject = new MenuItem("Script Object", menuItemScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
		var items = new[] { menuItemScriptObject }.ToReadOnlyCollection();
		var contextMenu = new ContextMenu(items);
		return contextMenu;
	}
}