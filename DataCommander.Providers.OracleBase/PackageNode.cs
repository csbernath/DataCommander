namespace DataCommander.Providers.OracleBase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

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

        public string Name
        {
            get
            {
                return name;
            }
        }

        public bool IsLeaf
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            //string commandText = "select distinct object_name from all_arguments where owner='{0}' and package_name='{1}'";
            //commandText = string.Format(commandText, schema.Name, name);
            string commandText = string.Format(@"select	procedure_name
from	all_procedures
where	owner = '{0}'
	and object_name = '{1}'
order by procedure_name", schemaNode.Name, name);
            var transactionScope = new DbTransactionScope(schemaNode.SchemasNode.Connection, null);

            return transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default, dataRecord =>
            {
                string procedureName = dataRecord.GetString(0);
                return new ProcedureNode(schemaNode, this, procedureName);
            });
        }

        public bool Sortable
        {
            get
            {
                return false;
            }
        }

        public string Query
        {
            get
            {
                string commandText = "select text from all_source where owner = '{0}' and type = 'PACKAGE' and name = '{1}'";
                commandText = String.Format(commandText, schemaNode.Name, name);
                var transactionScope = new DbTransactionScope(schemaNode.SchemasNode.Connection, null);
                DataTable dataTable = transactionScope.ExecuteDataTable(new CommandDefinition {CommandText = commandText});
                var sb = new StringBuilder();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    DataRow dataRow = dataTable.Rows[i];
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
            string commandText = "select text from all_source where owner = '{0}' and name = '{1}' and type = 'PACKAGE BODY'";
            commandText = string.Format(commandText, schemaNode.Name, name);
            var transactionScope = new DbTransactionScope(this.schemaNode.SchemasNode.Connection, null);
            DataTable dataTable = transactionScope.ExecuteDataTable(new CommandDefinition {CommandText = commandText});
            DataRowCollection dataRows = dataTable.Rows;
            int count = dataRows.Count;
            var sb = new StringBuilder();

            for (int i = 0; i < count; i++)
            {
                DataRow dataRow = dataRows[i];
                string line = (string)dataRow[0];
                sb.Append(line);
            }

            MainForm mainForm = DataCommanderApplication.Instance.MainForm;
            var queryForm = (QueryForm)mainForm.ActiveMdiChild;
            QueryTextBox tbQuery = queryForm.QueryTextBox;
            int selectionStart = tbQuery.RichTextBox.TextLength;

            string append = sb.ToString();

            tbQuery.RichTextBox.AppendText(append);
            tbQuery.RichTextBox.SelectionStart = selectionStart;
            tbQuery.RichTextBox.SelectionLength = append.Length;

            tbQuery.Focus();
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();

                ToolStripMenuItem menuItemPackage = new ToolStripMenuItem("Script Package", null, ScriptPackage);
                contextMenu.Items.Add(menuItemPackage);

                ToolStripMenuItem menuItemPackageBody = new ToolStripMenuItem("Script Package Body", null, ScriptPackageBody);
                contextMenu.Items.Add(menuItemPackageBody);

                return contextMenu;
            }
        }

        public SchemaNode SchemaNode
        {
            get
            {
                return schemaNode;
            }
        }

        readonly SchemaNode schemaNode;
        readonly string name;
    }
}