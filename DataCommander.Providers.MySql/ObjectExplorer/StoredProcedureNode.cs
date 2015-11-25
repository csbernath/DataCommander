namespace DataCommander.Providers.MySql
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using global::MySql.Data.MySqlClient;
    using DataCommander.Foundation.Data;

    internal sealed class StoredProcedureNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;
        private readonly string name;

        public StoredProcedureNode(DatabaseNode databaseNode, string name)
        {
            this.databaseNode = databaseNode;
            this.name = name;
        }

        string ITreeNode.Name
        {
            get
            {
                return this.name;
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return true;
            }
        }

        System.Collections.Generic.IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return null;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                var menu = new ContextMenuStrip();

                var item = new ToolStripMenuItem("Show create procedure", null, this.ShowCreateProcedure_Click);
                menu.Items.Add(item);

                return menu;
            }
        }

        private void ShowCreateProcedure_Click(object sender, EventArgs e)
        {
            string commandText = $"show create procedure {this.databaseNode.Name}.{this.name}";
            string statement = MySqlClientFactory.Instance.ExecuteReader(
                this.databaseNode.ObjectExplorer.ConnectionString,
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord => dataRecord.GetString(2)).First();

            Clipboard.SetText(statement);
            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.SetStatusbarPanelText("Copying create procedure statement to clipboard finished.", SystemColors.ControlText);
        }
    }
}