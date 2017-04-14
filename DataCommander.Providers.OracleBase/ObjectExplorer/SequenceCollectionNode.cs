namespace DataCommander.Providers.OracleBase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class SequenceCollectionNode : ITreeNode
    {
        private readonly SchemaNode schemaNode;

        public SequenceCollectionNode( SchemaNode schemaNode )
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
where	s.SEQUENCE_OWNER	= '{this.schemaNode.Name}'
order by s.SEQUENCE_NAME
";
            var transactionScope = new DbTransactionScope(schemaNode.SchemasNode.Connection, null);

            return transactionScope.ExecuteReader(new CommandDefinition {CommandText = commandText}, CommandBehavior.Default, dataRecord =>
            {
                var name = dataRecord.GetString(0);
                return (ITreeNode)new SequenceNode(this.schemaNode, name);
            });
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => throw new NotImplementedException();

        ContextMenuStrip ITreeNode.ContextMenu => throw new NotImplementedException();

        #endregion
    }
}