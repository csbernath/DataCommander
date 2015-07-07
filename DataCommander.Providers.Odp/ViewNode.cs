namespace DataCommander.Providers.Odp
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Oracle.ManagedDataAccess.Client;

    internal sealed class ViewNode : ITreeNode
	{
		private ViewCollectionNode parent;
		private string name;

		public ViewNode( ViewCollectionNode parent, string name )
		{
			this.parent = parent;
			this.name = name;
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool IsLeaf
		{
			get
			{
				return true;
			}
		}

		public IEnumerable<ITreeNode> GetChildren( bool refresh )
		{
			return null;
		}

		public bool Sortable
		{
			get
			{
				return false;
			}
		}

		public string Query
		{
			get
			{
				string query = string.Format( "select * from {0}.{1}", this.parent.SchemaNode.Name, name );
				return query;
			}
		}

		private void menuItemScriptObject_Click( object sender, EventArgs e )
		{
			string commandText = "select text from sys.all_views where owner = '{0}' and view_name = '{1}'";
			commandText = string.Format( commandText, this.parent.SchemaNode.Name, name );

			using (OracleCommand command = new OracleCommand( commandText, this.parent.SchemaNode.SchemasNode.Connection ))
			{
				command.InitialLONGFetchSize = 64 * 1024;

				using (OracleDataReader dataReader = command.ExecuteReader())
				{
					if (dataReader.Read())
					{
						string append = dataReader.GetString( 0 );

						MainForm mainForm = DataCommanderApplication.Instance.MainForm;
						QueryForm queryForm = (QueryForm) mainForm.ActiveMdiChild;
						QueryTextBox querytextBox = queryForm.QueryTextBox;
						int selectionStart = querytextBox.RichTextBox.TextLength;

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
				ToolStripMenuItem menuItemScriptObject = new ToolStripMenuItem( "Script Object", null, this.menuItemScriptObject_Click );
				ContextMenuStrip contextMenu = new ContextMenuStrip();
				contextMenu.Items.Add( menuItemScriptObject );
				return contextMenu;
			}
		}

		public void BeforeExpand()
		{
		}
	}
}