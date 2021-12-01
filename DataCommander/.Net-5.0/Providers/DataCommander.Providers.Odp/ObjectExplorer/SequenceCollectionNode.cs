using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    internal sealed class SequenceCollectionNode : ITreeNode
    {
        private readonly SchemaNode _schemaNode;

        public SequenceCollectionNode(SchemaNode schemaNode) => _schemaNode = schemaNode;

        #region ITreeNode Members

        string ITreeNode.Name => "Sequences";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText = $@"select	s.SEQUENCE_NAME
from	SYS.ALL_SEQUENCES s
where	s.SEQUENCE_OWNER	= '{_schemaNode.Name}'
order by s.SEQUENCE_NAME";
            var executor = _schemaNode.SchemasNode.Connection.CreateCommandExecutor();
            return executor.ExecuteReader(new ExecuteReaderRequest(commandText), 128, dataReader =>
            {
                var name = dataReader.GetString(0);
                return new SequenceNode(_schemaNode, name);
            });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => throw new NotImplementedException();
        ContextMenuStrip ITreeNode.ContextMenu => throw new NotImplementedException();
        public ContextMenu GetContextMenu()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}