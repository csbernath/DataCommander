namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;

    internal sealed class JobCollectionNode : ITreeNode
    {
        private readonly ServerNode server;

        public JobCollectionNode( ServerNode server )
        {
            Contract.Requires( server != null );
            this.server = server;
        }

        public ServerNode Server
        {
            get
            {
                return this.server;
            }
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Jobs";
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
            string commandText = @"select  j.name
from    msdb.dbo.sysjobs j (nolock)
order by j.name";
            using (var connection = new SqlConnection( this.Server.ConnectionString ))
            {
                connection.Open();
                using (var dataReader = connection.ExecuteReader( commandText ))
                {
                    foreach (var dataRecord in dataReader.AsEnumerable())
                    {
                        string name = dataRecord.GetString( 0 );
                        yield return new JobNode( this, name );
                    }
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