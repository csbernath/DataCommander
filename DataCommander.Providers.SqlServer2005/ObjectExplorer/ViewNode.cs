namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data.SqlClient;

    internal sealed class ViewNode : ITreeNode
    {
        private readonly DatabaseNode database;
        private readonly string schema;
        private readonly string name;

        public ViewNode(
            DatabaseNode database,
            string schema,
            string name)
        {
            this.database = database;
            this.schema = schema;
            this.name = name;
        }

        public string Name
        {
            get
            {
                return $"{this.schema}.{this.name}";
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new ColumnCollectionNode(this.database, this.schema, this.name),
                new TriggerCollectionNode(this.database, this.schema, this.name)
            };
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
                var name = new DatabaseObjectMultipartName(null, this.database.Name, this.schema, this.name);
                string connectionString = this.database.Databases.Server.ConnectionString;
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
                var menuItemScriptObject = new ToolStripMenuItem("Script Object", null, this.menuItemScriptObject_Click);
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add(menuItemScriptObject);
                return contextMenu;
            }
        }
    }
}