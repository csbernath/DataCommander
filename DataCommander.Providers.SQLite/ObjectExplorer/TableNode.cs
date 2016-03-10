namespace DataCommander.Providers.SQLite
{
    using System;
    using System.Collections.Generic;
    using System.Data.SQLite;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class TableNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;
        private readonly string name;

        public TableNode(DatabaseNode databaseNode, string name)
        {
            this.databaseNode = databaseNode;
            this.name = name;
        }

        public DatabaseNode Database => this.databaseNode;

        #region ITreeNode Members

        public string Name => this.name;

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            ITreeNode[] treeNodes = new ITreeNode[1];
            treeNodes[0] = new IndexCollectionNode(this);
            return treeNodes;
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => $"select\t*\r\nfrom\t{this.databaseNode.Name}.{this.name}";

        private static string GetScript(
            SQLiteConnection connection,
            string databaseName,
            string name)
        {
            string commandText = $@"
select  sql
from	{databaseName}.sqlite_master
where	name	= '{name}'";
            var transactionScope = new DbTransactionScope(connection, null);
            object scalar = transactionScope.ExecuteScalar(new CommandDefinition {CommandText = commandText});
            string script = Foundation.Data.Database.GetValueOrDefault<string>(scalar);
            return script;
        }

        private void Script_Click(object sender, EventArgs e)
        {
            string script = GetScript(this.databaseNode.Connection, this.databaseNode.Name, this.name);
            QueryForm.ShowText(script);
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                ContextMenuStrip contextMenu = null;

                if (this.name != "sqlite_master")
                {
                    contextMenu = new ContextMenuStrip();
                    contextMenu.Items.Add("Script", null, new EventHandler(this.Script_Click));
                }

                return contextMenu;
            }
        }

        #endregion
    }
}