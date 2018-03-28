using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    /// <summary>
    /// Summary description for TablesNode.
    /// </summary>
    internal sealed class PackageNode : ITreeNode
    {
        private readonly SchemaNode _schemaNode;
        private readonly string _name;

        public PackageNode(
            SchemaNode schema,
            string name)
        {
            _schemaNode = schema;
            _name = name;
        }

        public string Name => _name;
        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            //string commandText = "select distinct object_name from all_arguments where owner='{0}' and package_name='{1}'";
            //commandText = string.Format(commandText, schema.Name, name);
            var commandText =
                $@"select	procedure_name
from	all_procedures
where	owner = '{_schemaNode.Name}'
	and object_name = '{_name}'
order by procedure_name";
            var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();

            return executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataRecord =>
            {
                var procedureName = dataRecord.GetString(0);
                return new ProcedureNode(_schemaNode, this, procedureName);
            });
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var commandText = "select text from all_source where owner = '{0}' and type = 'PACKAGE' and name = '{1}'";
                commandText = string.Format(commandText, _schemaNode.Name, _name);
                var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();
                var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
                var sb = new StringBuilder();

                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    var dataRow = dataTable.Rows[i];
                    sb.Append(dataRow[0]);
                }

                return sb.ToString();
            }
        }

        private void ScriptPackage(object sender, EventArgs e)
        {
        }

        private void ScriptPackageBody(object sender, EventArgs e)
        {
            var commandText = "select text from all_source where owner = '{0}' and name = '{1}' and type = 'PACKAGE BODY'";
            commandText = string.Format(commandText, _schemaNode.Name, _name);
            var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();
            var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
            var dataRows = dataTable.Rows;
            var count = dataRows.Count;
            var sb = new StringBuilder();

            for (var i = 0; i < count; i++)
            {
                var dataRow = dataRows[i];
                var line = (string)dataRow[0];
                sb.Append(line);
            }

            var mainForm = DataCommanderApplication.Instance.MainForm;
            var queryForm = (QueryForm)mainForm.ActiveMdiChild;
            var tbQuery = queryForm.QueryTextBox;
            var selectionStart = tbQuery.RichTextBox.TextLength;

            var append = sb.ToString();

            tbQuery.RichTextBox.AppendText(append);
            tbQuery.RichTextBox.SelectionStart = selectionStart;
            tbQuery.RichTextBox.SelectionLength = append.Length;

            tbQuery.Focus();
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                var contextMenu = new ContextMenuStrip();

                var menuItemPackage = new ToolStripMenuItem("Script Package", null, ScriptPackage);
                contextMenu.Items.Add(menuItemPackage);

                var menuItemPackageBody = new ToolStripMenuItem("Script Package Body", null, ScriptPackageBody);
                contextMenu.Items.Add(menuItemPackageBody);

                return contextMenu;
            }
        }

        public void BeforeExpand()
        {
        }

        public SchemaNode SchemaNode => _schemaNode;
    }
}