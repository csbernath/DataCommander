using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Windows.Forms;
    using Query;

    internal sealed class ProcedureNode : ITreeNode
    {
		private readonly SchemaNode _schemaNode;
		private readonly PackageNode _packageNode;
		private readonly string _name;

        public ProcedureNode(
            SchemaNode schemaNode,
            PackageNode packageNode,
            string name)
        {
            _schemaNode = schemaNode;
            _packageNode = packageNode;
            _name = name;
        }

        public string Name => _name;

        public bool IsLeaf => true;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var query = "EXEC " + _schemaNode.Name + '.';

				if (_packageNode != null)
				{
					query += _packageNode.Name + '.';
				}

                query += _name;
                return query;
            }
        }

        private void ScriptObject_Click(object sender, EventArgs e)
        {
            var commandText =
                $@"select	text
from	all_source
where	owner = '{_schemaNode.Name}'
	and name = '{_name}'
	and type = 'PROCEDURE'
order by line";
            var sb = new StringBuilder();
            string text;
            var transactionScope = new DbTransactionScope(_schemaNode.SchemasNode.Connection, null);

            transactionScope.ExecuteReader(
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    text = dataRecord.GetString(0);
                    sb.Append(text);
                });

            text = sb.ToString();
            QueryForm.ShowText(text);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ContextMenuStrip contextMenu;

                if (_packageNode != null)
                {
                    contextMenu = null;
                }
                else
                {
                    var menuItem = new ToolStripMenuItem("Script Object", null, ScriptObject_Click);
                    contextMenu = new ContextMenuStrip();
                    contextMenu.Items.Add(menuItem);
                }

                return contextMenu;
            }
        }

        public void BeforeExpand()
        {
        }
    }
}