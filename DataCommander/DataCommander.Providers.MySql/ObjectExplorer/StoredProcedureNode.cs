using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using Foundation.Data;
using MySql.Data.MySqlClient;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    internal sealed class StoredProcedureNode : ITreeNode
    {
        private readonly DatabaseNode _databaseNode;
        private readonly string _name;

        public StoredProcedureNode(DatabaseNode databaseNode, string name)
        {
            this._databaseNode = databaseNode;
            this._name = name;
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

                var item = new ToolStripMenuItem("Show create procedure", null, ShowCreateProcedure_Click);
                menu.Items.Add(item);

                return menu;
            }
        }

        private void ShowCreateProcedure_Click(object sender, EventArgs e)
        {
            var commandText = $"show create procedure {_databaseNode.Name}.{_name}";
            var statement = MySqlClientFactory.Instance.ExecuteReader(
                _databaseNode.ObjectExplorer.ConnectionString,
                new ExecuteReaderRequest(commandText),
                dataRecord => dataRecord.GetString(2)).First();

            Clipboard.SetText(statement);
            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.SetStatusbarPanelText("Copying create procedure statement to clipboard finished.", SystemColors.ControlText);
        }
    }
}