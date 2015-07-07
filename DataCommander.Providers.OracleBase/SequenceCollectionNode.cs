namespace DataCommander.Providers.OracleBase
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class SequenceCollectionNode : ITreeNode
    {
        private SchemaNode schemaNode;

        public SequenceCollectionNode( SchemaNode schemaNode )
        {
            this.schemaNode = schemaNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Sequences";
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return false;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren( bool refresh )
        {
            string commandText = string.Format( @"select	s.SEQUENCE_NAME
from	SYS.ALL_SEQUENCES s
where	s.SEQUENCE_OWNER	= '{0}'
order by s.SEQUENCE_NAME
", this.schemaNode.Name );

            using (var context = this.schemaNode.SchemasNode.Connection.ExecuteReader( null, commandText, CommandType.Text, 0, CommandBehavior.Default ))
            {
                var dataReader = context.DataReader;
                while (dataReader.Read())
                {
                    string name = dataReader.GetString( 0 );
                    yield return new SequenceNode( this.schemaNode, name );
                }
            }
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

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