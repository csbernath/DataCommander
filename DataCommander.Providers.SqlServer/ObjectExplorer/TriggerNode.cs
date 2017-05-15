namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Windows.Forms;
    using Foundation.Data;
    using Query;

    internal sealed class TriggerNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;
        private readonly int id;
        private readonly string name;

        public TriggerNode(DatabaseNode databaseNode, int id, string name)
        {
            this.databaseNode = databaseNode;
            this.id = id;
            this.name = name;
        }

        public string Name => this.name;
        public bool IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query => null;

        void menuItemScriptObject_Click(object sender, EventArgs e)
        {
            var cb = new SqlCommandBuilder();
            var databaseName = cb.QuoteIdentifier(this.databaseNode.Name);

            var commandText = $@"select m.definition
from {databaseName}.sys.sql_modules m (nolock)
where m.object_id = {this.id}";

            var connectionString = this.databaseNode.Databases.Server.ConnectionString;
            string definition;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var trasnactionScope = new DbTransactionScope(connection, null);
                definition = (string)trasnactionScope.ExecuteScalar(new CommandDefinition {CommandText = commandText});
            }
            QueryForm.ShowText(definition);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                var menuItemScriptObject = new ToolStripMenuItem("Script Object", null, new EventHandler(this.menuItemScriptObject_Click));
                var contextMenu = new ContextMenuStrip();
                contextMenu.Items.Add(menuItemScriptObject);
                return contextMenu;
            }
        }
    }
}