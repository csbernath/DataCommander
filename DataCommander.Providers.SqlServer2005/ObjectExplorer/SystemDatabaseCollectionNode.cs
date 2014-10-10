namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Data;
    using DataCommander.Providers;

    internal sealed class SystemDatabaseCollectionNode : ITreeNode
    {
        private DatabaseCollectionNode databaseCollectionNode;

        public SystemDatabaseCollectionNode( DatabaseCollectionNode databaseCollectionNode )
        {
            Contract.Requires( databaseCollectionNode != null );
            this.databaseCollectionNode = databaseCollectionNode;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "System Databases";
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
            string connectionString = this.databaseCollectionNode.Server.ConnectionString;
            DataTable dataTable;
            using (var connection = new SqlConnection( connectionString ))
            {
                const string commandText = @"select d.name
from sys.databases d (nolock)
where name in('master','model','msdb','tempdb')
order by d.name";
                dataTable = connection.ExecuteDataTable( null, commandText, CommandType.Text, 0 );
            }

            List<ITreeNode> list = new List<ITreeNode>();
            foreach (DataRow dataRow in dataTable.Rows)
            {
                string name = (string) dataRow[ 0 ];
                DatabaseNode node = new DatabaseNode( this.databaseCollectionNode, name );
                list.Add( node );
            }

            return list;
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