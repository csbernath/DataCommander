﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using DataCommander.Providers.Query;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    internal sealed class ProcedureNode : ITreeNode
    {
		private readonly SchemaNode _schemaNode;
		private readonly PackageNode _packageNode;
		private readonly string _name;

        public ProcedureNode(SchemaNode schemaNode, PackageNode packageNode, string name)
        {
            _schemaNode = schemaNode;
            _packageNode = packageNode;
            _name = name;
        }

        public string Name => _name;
        public bool IsLeaf => true;
        public IEnumerable<ITreeNode> GetChildren(bool refresh) => null;
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
            var commandText = $@"select	text
from all_source
where owner = {_schemaNode.Name.ToVarChar()}
    and name = {_name.ToVarChar()}
    and type = 'PROCEDURE'
order by line";
            var stringBuilder = new StringBuilder();
            var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();

            executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
            {
                while (dataReader.Read())
                {
                    var s = dataReader.GetString(0);
                    stringBuilder.Append(s);
                }
            });

            var text = stringBuilder.ToString();
            QueryForm.ShowText(text);
        }

        public ContextMenuStrip ContextMenu
        {
            get
            {
                ContextMenuStrip contextMenu;

                if (_packageNode != null)
                    contextMenu = null;
                else
                {
                    var menuItem = new ToolStripMenuItem("Script Object", null, ScriptObject_Click);
                    contextMenu = new ContextMenuStrip();
                    contextMenu.Items.Add(menuItem);
                }

                return contextMenu;
            }
        }
    }
}