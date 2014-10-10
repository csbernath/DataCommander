namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;
    using DataCommander.Providers;

    internal sealed class LinkedServerCollectionNode : ITreeNode
    {
        private ServerNode server;

        public LinkedServerCollectionNode( ServerNode serverNode )
        {
            Contract.Requires( serverNode != null );
            this.server = serverNode;
        }

        public ServerNode Server
        {
            get
            {
                return server;
            }
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Linked Servers";
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
            string commandText = @"select  s.name
from    sys.servers s (nolock)
where   s.is_linked = 1
order by s.name";
            ITreeNode[] childNodes;
            using (var connection = new SqlConnection(this.server.ConnectionString))
            {
                connection.Open();
                using (var dataReader = connection.ExecuteReader( commandText ))
                {
                    childNodes =
                        (from dataRecord in dataReader.AsEnumerable()
                         select new LinkedServerNode( this, dataReader.GetString( 0 ) )).ToArray();
                }
            }

            return childNodes;
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

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                return null;
            }
        }

        #endregion
    }
}