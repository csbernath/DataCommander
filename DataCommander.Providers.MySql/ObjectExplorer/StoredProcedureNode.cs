﻿using Foundation.Data;

namespace DataCommander.Providers.MySql.ObjectExplorer
{
    using System;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;
    using global::MySql.Data.MySqlClient;
    using Query;

    internal sealed class StoredProcedureNode : ITreeNode
    {
        private readonly DatabaseNode databaseNode;
        private readonly string name;

        public StoredProcedureNode(DatabaseNode databaseNode, string name)
        {
            this.databaseNode = databaseNode;
            this.name = name;
        }

        string ITreeNode.Name => name;

        bool ITreeNode.IsLeaf => true;

        System.Collections.Generic.IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

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
            var commandText = $"show create procedure {databaseNode.Name}.{name}";
            var statement = MySqlClientFactory.Instance.ExecuteReader(
                databaseNode.ObjectExplorer.ConnectionString,
                new ExecuteReaderRequest(commandText),
                dataRecord => dataRecord.GetString(2)).First();

            Clipboard.SetText(statement);
            var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
            queryForm.SetStatusbarPanelText("Copying create procedure statement to clipboard finished.", SystemColors.ControlText);
        }
    }
}