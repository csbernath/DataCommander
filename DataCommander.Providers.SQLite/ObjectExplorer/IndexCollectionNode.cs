namespace DataCommander.Providers.SQLite
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Data;

    internal sealed class IndexCollectionNode : ITreeNode
    {
        private TableNode tableNode;

        public IndexCollectionNode(TableNode tableNode)
        {
            Contract.Requires(tableNode != null);

            this.tableNode = tableNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Indexes";
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
            //            string commandText = string.Format(@"select	idx
            //from	main.sqlite_stat1
            //where	tbl = '{0}'
            //order by idx", this.tableNode.Name);
            string commandText = string.Format( "PRAGMA index_list({0});", this.tableNode.Name );
            List<ITreeNode> children = new List<ITreeNode>();

            using (var context = this.tableNode.Database.Connection.ExecuteReader( null, commandText, CommandType.Text, 0, CommandBehavior.Default ))
            {
                var dataReader = context.DataReader;
                if (dataReader.Read())
                {
                    string name = Database.GetValueOrDefault<string>( dataReader[ "Name" ] );
                    IndexNode indexNode = new IndexNode( this.tableNode, name );
                    children.Add( indexNode );
                }
            }

            return children;
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
                return null;
            }
        }

        System.Windows.Forms.ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}