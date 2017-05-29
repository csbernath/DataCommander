namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Oracle.ManagedDataAccess.Client;
    using Query;

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

						var mainForm = DataCommanderApplication.Instance.MainForm;
						var queryForm = (QueryForm) mainForm.ActiveMdiChild;
						var querytextBox = queryForm.QueryTextBox;
						var selectionStart = querytextBox.RichTextBox.TextLength;

						querytextBox.RichTextBox.AppendText( append );
						querytextBox.RichTextBox.SelectionStart = selectionStart;
						querytextBox.RichTextBox.SelectionLength = append.Length;

						querytextBox.Focus();
					}

					dataReader.Close();
				}
			}
		}

		public ContextMenuStrip ContextMenu
		{
			get
			{
				var menuItemScriptObject = new ToolStripMenuItem( "Script Object", null, menuItemScriptObject_Click );
				var contextMenu = new ContextMenuStrip();
				contextMenu.Items.Add( menuItemScriptObject );
				return contextMenu;
			}
		}

		public void BeforeExpand()
		{
		}
	}
}