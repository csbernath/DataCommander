namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Foundation.Data.SqlClient;
    using Query;

    internal sealed class ViewNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private readonly int id;
        private readonly string schema;
        private readonly string name;

        public ViewNode(DatabaseNode database, int id, string schema, string name)
        {
            this.database = database;
            this.id = id;
            this.schema = schema;
            this.name = name;
        }

        public string Name => $"{this.schema}.{this.name}";

        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new ColumnCollectionNode(this.database, this.id),
                new TriggerCollectionNode(this.database, this.id)
            };
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var name = new DatabaseObjectMultipartName(null, this.database.Name, this.schema, this.name);
                var connectionString = this.database.Databases.Server.ConnectionString;
                string text;
                using (var connection = new SqlConnection(connectionString))
                {
                    text = TableNode.GetSelectStatement(connection, name);
                }
                return text;
            }
        }

        private void menuItemScriptObject_Click(object sender, EventArgs e)
        {
            var connectionString = this.database.Databases.Server.ConnectionString;
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
                var menuItemScriptObject = new ToolStripMenuItem("Script Object", null, this.menuItemScriptObject_Click);
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add(menuItemScriptObject);
                return contextMenu;
            }
        }
    }
}