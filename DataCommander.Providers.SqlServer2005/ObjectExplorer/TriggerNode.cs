namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Foundation.Data.SqlClient;

    internal sealed class TriggerNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private string schema;
        private string objectName;

        public TriggerNode(DatabaseNode database, string schema, string objectName, string name)
        {
            this.database = database;
            this.schema = schema;
            this.objectName = objectName;
            this.Name = name;
        }

        public string Name { get; }

        public bool IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query => null;

        void menuItemScriptObject_Click(object sender, EventArgs e)
        {
            string connectionString = this.database.Databases.Server.ConnectionString;
            string text;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                text = SqlDatabase.GetSysComments(connection, this.database.Name, this.schema, this.Name);
            }
            QueryForm.ShowText(text);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ToolStripMenuItem menuItemScriptObject = new ToolStripMenuItem("Script Object", null, new EventHandler(this.menuItemScriptObject_Click));
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add(menuItemScriptObject);
                return contextMenu;
            }
        }
    }
}