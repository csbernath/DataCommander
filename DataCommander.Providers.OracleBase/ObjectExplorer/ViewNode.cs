namespace DataCommander.Providers.OracleBase
{
    using System;
    using System.Collections.Generic;
    using System.Windows.Forms;
    using Query;

    public sealed class ViewNode : ITreeNode
    {
        private readonly ViewCollectionNode parent;
        private readonly string name;

        public ViewNode(ViewCollectionNode parent, string name)
        {
            this.parent = parent;
            this.name = name;
        }

        public string Name => this.name;

        public bool IsLeaf => true;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var query = $"select * from {this.parent.SchemaNode.Name}.{name}";
                return query;
            }
        }

        private void menuItemScriptObject_Click(object sender, EventArgs e)
        {
            var commandText = "select text from sys.all_views where owner = '{0}' and view_name = '{1}'";
            commandText = string.Format(commandText, this.parent.SchemaNode.Name, name);

            using (var command = this.parent.SchemaNode.SchemasNode.Connection.CreateCommand())
            {
                command.CommandText = commandText;
                //  TODO
                // command.InitialLONGFetchSize = 64 * 1024;

                using (var dataReader = command.ExecuteReader())
                {
                    if (dataReader.Read())
                    {
                        var append = dataReader.GetString(0);

                        var mainForm = DataCommanderApplication.Instance.MainForm;
                        var queryForm = (QueryForm)mainForm.ActiveMdiChild;
                        var querytextBox = queryForm.QueryTextBox;
                        var selectionStart = querytextBox.RichTextBox.TextLength;

                        querytextBox.RichTextBox.AppendText(append);
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
                var menuItemScriptObject = new ToolStripMenuItem("Script Object", null, this.menuItemScriptObject_Click);
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add(menuItemScriptObject);
                return contextMenu;
            }
        }
    }
}