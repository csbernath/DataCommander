using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class SynonymCollectionNode : ITreeNode
    {
        public SynonymCollectionNode(SchemaNode schema)
        {
            this._schema = schema;
        }

        public string Name => "Synonyms";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var commandText = @"select	s.SYNONYM_NAME
from	SYS.ALL_SYNONYMS s
where	s.OWNER	= '{0}'
order by s.SYNONYM_NAME";
            commandText = string.Format(commandText, _schema.Name);
            var executor = _schema.SchemasNode.Connection.CreateCommandExecutor();
            var dataTable = executor.ExecuteDataTable(new ExecuteReaderRequest(commandText));
            var count = dataTable.Rows.Count;

            var treeNodes = new ITreeNode[count];

            for (var i = 0; i < count; i++)
            {
                var name = (string) dataTable.Rows[i][0];
                treeNodes[i] = new SynonymNode(_schema, name);
            }

            return treeNodes;
        }

        public bool Sortable => false;
        public string Query => null;
        public SchemaNode Schema => _schema;
        public ContextMenuStrip ContextMenu => null;
        private readonly SchemaNode _schema;
    }
}