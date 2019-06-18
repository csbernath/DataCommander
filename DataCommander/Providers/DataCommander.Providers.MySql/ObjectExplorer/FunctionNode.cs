using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using Foundation.Data;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    internal sealed class FunctionNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;
        private readonly string _name;

        public FunctionNode(DatabaseNode databaseNode, string name)
        {
            _databaseNode = databaseNode;
            _name = name;
        }

        string ITreeNode.Name => _name;
        bool ITreeNode.IsLeaf => true;

        System.Collections.Generic.IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => null;

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                var menu = new ContextMenuStrip();

                var item = new ToolStripMenuItem("Show create function", null, ShowCreateFunction_Click);
                menu.Items.Add(item);

                return menu;
            }
        }

        private void ShowCreateFunction_Click(object sender, EventArgs e)
        {
            var commandText = $"show create function {_databaseNode.Name}.{_name}";
            var statement = MySqlClientFactory.Instance.ExecuteReader(
                    _databaseNode.ObjectExplorer.ConnectionString,
                    new ExecuteReaderRequest(commandText),
                    128,
                    dataRecord => dataRecord.GetString(2))
                .First();

            Clipboard.SetText(statement);
            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.SetStatusbarPanelText("Copying create function statement to clipboard finished.", SystemColors.ControlText);
        }
    }
}