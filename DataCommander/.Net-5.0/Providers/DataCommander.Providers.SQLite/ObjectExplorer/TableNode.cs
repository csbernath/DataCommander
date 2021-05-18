using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using Foundation.Data;

namespace DataCommander.Providers.SQLite.ObjectExplorer
{
    internal sealed class TableNode : ITreeNode
    {
        public TableNode(DatabaseNode databaseNode, string name)
        {
            Database = databaseNode;
            Name = name;
        }

        public DatabaseNode Database { get; }

        #region ITreeNode Members

        public string Name { get; }

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var treeNodes = new ITreeNode[1];
            treeNodes[0] = new IndexCollectionNode(this);
            return treeNodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => $"select\t*\r\nfrom\t{Database.Name}.{Name}";

        private static string GetScript(
            SQLiteConnection connection,
            string databaseName,
            string name)
        {
            var commandText = $@"
select  sql
from	{databaseName}.sqlite_master
where	name	= '{name}'";
            var executor = DbCommandExecutorFactory.Create(connection);
            var scalar = executor.ExecuteScalar(new CreateCommandRequest(commandText));
            var script = (string) scalar;
            return script;
        }

        private void Script_Click(object sender, EventArgs e)
        {
            var script = GetScript(Database.Connection, Database.Name, Name);
            QueryForm.ShowText(script);
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                ContextMenuStrip contextMenu = null;

                if (Name != "sqlite_master")
                {
                    contextMenu = new ContextMenuStrip();
                    contextMenu.Items.Add("Script", null, Script_Click);
                }

                return contextMenu;
            }
        }

        #endregion
    }
}