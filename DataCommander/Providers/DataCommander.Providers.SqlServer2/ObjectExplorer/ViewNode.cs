using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer2.ObjectExplorer
{
    internal sealed class ViewNode : ITreeNode
    {
        private readonly DatabaseNode _database;
        private readonly int _id;
        private readonly string _name;
        private readonly string _schema;

        public ViewNode(DatabaseNode database, int id, string schema, string name)
        {
            _database = database;
            _id = id;
            _schema = schema;
            _name = name;
        }

        public string Name => $"{_schema}.{_name}";
        public bool IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return new ITreeNode[]
            {
                new ColumnCollectionNode(_database, _id),
                new TriggerCollectionNode(_database, _id),
                new IndexCollectionNode(_database, _id)
            };
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var name = new DatabaseObjectMultipartName(null, _database.Name, _schema, _name);
                var connectionString = _database.Databases.Server.ConnectionString;
                string text;
                using (var connection = new SqlConnection(connectionString))
                    text = TableNode.GetSelectStatement(connection, name);
                return text;
            }
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                var menuItemScriptObject = new ToolStripMenuItem("Script Object", null, menuItemScriptObject_Click);
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add(menuItemScriptObject);
                return contextMenu;
            }
        }

        private void menuItemScriptObject_Click(object sender, EventArgs e)
        {
            var connectionString = _database.Databases.Server.ConnectionString;
            string text;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                text = SqlDatabase.GetSysComments(connection, _database.Name, _schema, _name);
            }

            QueryForm.ShowText(text);
        }
    }
}