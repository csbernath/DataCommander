using Foundation.Data;

namespace DataCommander.Providers.OracleBase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using Query;

    /// <summary>
    /// Summary description for TablesNode.
    /// </summary>
    public sealed class PackageNode : ITreeNode
    {
        public PackageNode(
          SchemaNode schema,
          string name)
        {
            this.schemaNode = schema;
            this.name = name;
        }

        public string Name => name;

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            //string commandText = "select distinct object_name from all_arguments where owner='{0}' and package_name='{1}'";
            //commandText = string.Format(commandText, schema.Name, name);
            var commandText =
                $@"select	procedure_name
from	all_procedures
where	owner = '{schemaNode.Name}'
	and object_name = '{name}'
order by procedure_name";
            var transactionScope = new DbTransactionScope(schemaNode.SchemasNode.Connection, null);

            return transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default, dataRecord =>
            {
                var procedureName = dataRecord.GetString(0);
                return new ProcedureNode(schemaNode, this, procedureName);
            });
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var commandText = "select text from all_source where owner = '{0}' and type = 'PACKAGE' and name = '{1}'";
                commandText = string.Format(commandText, schemaNode.Name, name);
                var transactionScope = new DbTransactionScope(schemaNode.SchemasNode.Connection, null);
                var dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
                var sb = new StringBuilder();

                for (var i = 0; i < dataTable.Rows.Count; i++)
                {
                    var dataRow = dataTable.Rows[i];
                    sb.Append(dataRow[0]);
                }

                return sb.ToString();
            }
        }

        void ScriptPackage(object sender, EventArgs e)
        {
        }

        void ScriptPackageBody(object sender, EventArgs e)
        {
            var commandText = "select text from all_source where owner = '{0}' and name = '{1}' and type = 'PACKAGE BODY'";
            commandText = string.Format(commandText, schemaNode.Name, name);
            var transactionScope = new DbTransactionScope(this.schemaNode.SchemasNode.Connection, null);
            var dataTable = transactionScope.ExecuteDataTable(new CommandDefinition { CommandText = commandText }, CancellationToken.None);
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

        public SchemaNode SchemaNode => schemaNode;

        readonly SchemaNode schemaNode;
        readonly string name;
    }
}