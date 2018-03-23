using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Foundation.Data;

namespace DataCommander.Providers.OracleBase.ObjectExplorer
{
    internal sealed class SequenceCollectionNode : ITreeNode
    {
        private readonly SchemaNode schemaNode;

        public SequenceCollectionNode(SchemaNode schemaNode)
        {
            this.schemaNode = schemaNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Sequences";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"select	s.SEQUENCE_NAME
from	SYS.ALL_SEQUENCES s
where	s.SEQUENCE_OWNER	= '{schemaNode.Name}'
order by s.SEQUENCE_NAME
";
            var executor = schemaNode.SchemasNode.Connection.CreateCommandExecutor();

            return executor.ExecuteReader(new ExecuteReaderRequest(commandText), dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return (ITreeNode) new SequenceNode(schemaNode, name);
            });
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => throw new NotImplementedException();
        ContextMenuStrip ITreeNode.ContextMenu => throw new NotImplementedException();

        #endregion
    }
}