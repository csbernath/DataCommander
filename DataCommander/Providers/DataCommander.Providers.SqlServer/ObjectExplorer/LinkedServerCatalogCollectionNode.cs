using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
    internal sealed class LinkedServerCatalogCollectionNode : ITreeNode
    {
        private readonly LinkedServerNode _linkedServer;

        public LinkedServerCatalogCollectionNode(LinkedServerNode linkedServer)
        {
            Assert.IsNotNull(linkedServer);
            _linkedServer = linkedServer;
        }

        #region ITreeNode Members

        string ITreeNode.Name => "Catalogs";
        bool ITreeNode.IsLeaf => false;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            const string commandText = @"declare @provider nvarchar(128)
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

            using (var connection = new SqlConnection(_linkedServer.LinkedServers.Server.ConnectionString))
            {
                connection.Open();

                var parameters = new SqlParameterCollectionBuilder();
                parameters.Add("@name", _linkedServer.Name);
                parameters.Add("@getSystemCatalogs", false);

                var executor = connection.CreateCommandExecutor();
                var executeReaderRequest = new ExecuteReaderRequest(commandText, parameters.ToReadOnlyCollection());
                return executor.ExecuteReader(executeReaderRequest, 128,
                    dataRecord => new LinkedServerCatalogNode(_linkedServer, dataRecord.GetString(0)));
            }
        }

        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => null;
        ContextMenuStrip ITreeNode.ContextMenu => null;

        #endregion
    }
}