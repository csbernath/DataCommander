namespace DataCommander.Providers.SqlServer2005
{
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Windows.Forms;
    using DataCommander.Foundation.Data;
    using DataCommander.Providers;

    internal sealed class LinkedServerCatalogCollectionNode : ITreeNode
    {
        private LinkedServerNode linkedServer;

        public LinkedServerCatalogCollectionNode( LinkedServerNode linkedServer )
        {
            Contract.Requires( linkedServer != null );
            this.linkedServer = linkedServer;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return "Catalogs";
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
            string commandText = @"declare @provider nvarchar(128)
select  @provider = s.provider
from    sys.servers s (nolock)
where   s.name = @name

create table #catalog
(
    CATALOG_NAME    nvarchar(128),
    DESCRIPTION     nvarchar(255)
)

insert into #catalog execute sp_catalogs 'UKSECPRSD98\EUR'

if @provider = 'SQLNCLI'
begin
    select  c.CATALOG_NAME
    from    #catalog c
    where
        (@getSystemCatalogs = 0 and c.CATALOG_NAME not in('master','model','msdb','tempdb'))
        or
        (@getSystemCatalogs = 1 and c.CATALOG_NAME in('master','model','msdb','tempdb'))
    order by c.CATALOG_NAME
end
else
begin
    select  c.CATALOG_NAME
    from    #catalog c
    order by c.CATALOG_NAME
end

drop table #catalog";

            ITreeNode[] childNodes;
            using (var connection = new SqlConnection( this.linkedServer.LinkedServers.Server.ConnectionString ))
            {
                connection.Open();

                var command = connection.CreateCommand();
                command.CommandText = commandText;
                var parameters = command.Parameters;

                var parameter = new SqlParameter( "@name", SqlDbType.NVarChar, 128 );
                parameter.Value = this.linkedServer.Name;
                parameters.Add( parameter );
                parameters.Add( new SqlParameter( "@getSystemCatalogs", false ) );

                using (var dataReader = command.ExecuteReader())
                {
                    childNodes =
                        (from dataRecord in dataReader.AsEnumerable()
                         select new LinkedServerCatalogNode( this.linkedServer, dataReader.GetString( 0 ) )).ToArray();
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