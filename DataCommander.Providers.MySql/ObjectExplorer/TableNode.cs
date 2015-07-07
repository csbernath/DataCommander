namespace DataCommander.Providers.MySql
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Drawing;
    using System.Windows.Forms;
    using global::MySql.Data.MySqlClient;
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

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return string.Format(@"select *
from {0}.{1}", this.databaseNode.Name, this.name);
            }
        }

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                var menu = new ContextMenuStrip();

                var item = new ToolStripMenuItem("Show create table", null, this.ShowCreateTable_Click);
                menu.Items.Add(item);

                return menu;
            }
        }

        private void ShowCreateTable_Click(object sender, EventArgs e)
        {
            string commandText = string.Format("show create table {0}.{1}", this.databaseNode.Name, this.name);
            string createTableStatement = null;

            using (var connection = new MySqlConnection(this.databaseNode.ObjectExplorer.ConnectionString))
            {
                connection.Open();
                using (var context = connection.ExecuteReader(null, commandText, CommandType.Text, 0, CommandBehavior.Default))
                {
                    var dataReader = context.DataReader;
                    while (dataReader.Read())
                    {
                        createTableStatement = dataReader.GetString(1);
                    }
                }
            }

            Clipboard.SetText(createTableStatement);
            var queryForm = (QueryForm)DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.SetStatusbarPanelText("Copying create table statement to clipboard finished.", SystemColors.ControlText);
        }
    }
}