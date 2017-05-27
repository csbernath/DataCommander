using Foundation.Data;

namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;

    internal sealed class SequenceCollectionNode : ITreeNode
    {
        private readonly SchemaNode _schemaNode;

        public SequenceCollectionNode(SchemaNode schemaNode)
        {
            _schemaNode = schemaNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Sequences";

        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            var commandText =
                $@"select	s.SEQUENCE_NAME
from	SYS.ALL_SEQUENCES s
where	s.SEQUENCE_OWNER	= '{_schemaNode.Name}'
order by s.SEQUENCE_NAME
";
            var transactionScope = new DbTransactionScope(_schemaNode.SchemasNode.Connection, null);

            return transactionScope.ExecuteReader(
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new SequenceNode(_schemaNode, name);
                });
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => throw new NotImplementedException();

        ContextMenuStrip ITreeNode.ContextMenu => throw new NotImplementedException();

        #endregion
    }
}