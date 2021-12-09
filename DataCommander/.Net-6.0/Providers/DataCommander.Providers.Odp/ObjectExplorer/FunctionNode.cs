using System;
using System.Collections.Generic;
using System.Text;
using DataCommander.Providers.Query;
using Foundation.Collections.ReadOnly;
using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    internal sealed class FunctionNode : ITreeNode
    {
        private readonly SchemaNode _schemaNode;
        private readonly PackageNode _packageNode;
        private readonly string _name;

        public FunctionNode(SchemaNode schemaNode, PackageNode packageNode, string name)
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
	and type = 'FUNCTION'
order by line";
            var sb = new StringBuilder();
            string text;
            var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();

            executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataReader =>
            {
                while (dataReader.Read())
                {
                    text = dataReader.GetString(0);
                    sb.Append(text);
                }
            });

            text = sb.ToString();
            QueryForm.ShowText(text);
        }

        public ContextMenu GetContextMenu()
        {
            ContextMenu contextMenu;

            if (_packageNode != null)
                contextMenu = null;
            else
            {
                var menuItem = new MenuItem("Script Object", ScriptObject_Click, EmptyReadOnlyCollection<MenuItem>.Value);
                var items = new[] { menuItem }.ToReadOnlyCollection();
                contextMenu = new ContextMenu(items);
            }

            return contextMenu;
        }
    }
}