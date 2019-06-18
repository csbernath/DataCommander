using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    internal sealed class ProcedureCollectionNode : ITreeNode
    {
        public ProcedureCollectionNode(SchemaNode schemaNode)
        {
            _schemaNode = schemaNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Procedures";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"select	OBJECT_NAME
from	SYS.ALL_OBJECTS
where	OWNER	= '{_schemaNode.Name}'
	and OBJECT_TYPE	= 'PROCEDURE'
order by OBJECT_NAME";
            var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();

            return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataRecord =>
            {
                var procedureName = dataRecord.GetString(0);
                return new ProcedureNode(_schemaNode, null, procedureName);
            });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion

        private readonly SchemaNode _schemaNode;
    }
}