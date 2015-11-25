namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class TriggerNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private string schema;
        private string objectName;
        private readonly string name;

        public TriggerNode(DatabaseNode database, string schema, string objectName, string name)
        {
            this.database = database;
            this.schema = schema;
            this.objectName = objectName;
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

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
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
                return null;
            }
        }

        void menuItemScriptObject_Click(object sender, EventArgs e)
        {
            string connectionString = this.database.Databases.Server.ConnectionString;
            string text;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                text = SqlDatabase.GetSysComments(connection, this.database.Name, this.schema, this.name);
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