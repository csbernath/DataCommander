using Foundation.Data;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using global::MySql.Data.MySqlClient;
    using Query;

    internal sealed class TableNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;
        private readonly string name;

        public TableNode(DatabaseNode databaseNode, string name)
        {
            this.databaseNode = databaseNode;
            this.name = name;
        }

        string ITreeNode.Name => name;

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable => throw new NotImplementedException();

        string ITreeNode.Query => $@"select *
from {databaseNode.Name}.{name}";

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                var menu = new ContextMenuStrip();

                var item = new ToolStripMenuItem("Show create table", null, ShowCreateTable_Click);
                menu.Items.Add(item);

                return menu;
            }
        }

        private void ShowCreateTable_Click(object sender, EventArgs e)
        {
            var commandText = $"show create table {databaseNode.Name}.{name}";
            var createTableStatement = MySqlClientFactory.Instance.ExecuteReader(
                databaseNode.ObjectExplorer.ConnectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord => dataRecord.GetString(1)).First();

            Clipboard.SetText(createTableStatement);
            var queryForm = (QueryForm)DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.SetStatusbarPanelText("Copying create table statement to clipboard finished.", SystemColors.ControlText);
        }
    }
}