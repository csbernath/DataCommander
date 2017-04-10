namespace DataCommander.Providers.Odp.ObjectExplorer
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using Foundation.Data;

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
            string commandText =
                $@"select	s.SEQUENCE_NAME
from	SYS.ALL_SEQUENCES s
where	s.SEQUENCE_OWNER	= '{this.schemaNode.Name}'
order by s.SEQUENCE_NAME
";
            var transactionScope = new DbTransactionScope(this.schemaNode.SchemasNode.Connection, null);

            return transactionScope.ExecuteReader(
                new CommandDefinition {CommandText = commandText},
                CommandBehavior.Default,
                dataRecord =>
                {
                    var name = dataRecord.GetString(0);
                    return new SequenceNode(this.schemaNode, name);
                });
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}