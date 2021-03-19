using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using Foundation.Data;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    internal sealed class TableNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;
        private readonly string _name;

        public TableNode(DatabaseNode databaseNode, string name)
        {
            _databaseNode = databaseNode;
            _name = name;
        }

        string ITreeNode.Name => _name;
        bool ITreeNode.IsLeaf => true;
        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => throw new NotImplementedException();
        bool ITreeNode.Sortable => throw new NotImplementedException();
        string ITreeNode.Query => $@"select * from {_databaseNode.Name}.{_name}";

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
            var commandText = $"show create table {_databaseNode.Name}.{_name}";
            var createTableStatement = MySqlClientFactory.Instance.ExecuteReader(
                _databaseNode.ObjectExplorer.ConnectionString,
                new ExecuteReaderRequest(commandText),
                128,
                dataRecord => dataRecord.GetString(1)).First();

            Clipboard.SetText(createTableStatement);
            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.SetStatusbarPanelText("Copying create table statement to clipboard finished.", SystemColors.ControlText);
        }
    }
}