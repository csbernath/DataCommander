using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class LinkedServerCatalogCollectionNode : ITreeNode
{
    private readonly LinkedServerNode _linkedServer;

    public LinkedServerCatalogCollectionNode(LinkedServerNode linkedServer)
    {
        ArgumentNullException.ThrowIfNull(linkedServer);
        _linkedServer = linkedServer;
    }

    string? ITreeNode.Name => "Catalogs";
    bool ITreeNode.IsLeaf => false;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
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

        using var connection = _linkedServer.LinkedServers.Server.CreateConnection();
        connection.Open();

        var parameters = new SqlParameterCollectionBuilder();
        parameters.Add("@name", _linkedServer.Name!);
        parameters.Add("@getSystemCatalogs", false);

        var executor = connection.CreateCommandExecutor();
        var executeReaderRequest = new ExecuteReaderRequest(commandText, parameters.ToReadOnlyCollection());
        return Task.FromResult<IEnumerable<ITreeNode>>(executor.ExecuteReader(executeReaderRequest, 128,
            dataRecord => new LinkedServerCatalogNode(_linkedServer, dataRecord.GetString(0))));
    }

    bool ITreeNode.Sortable => false;
    string? ITreeNode.Query => null;

    public ContextMenu? GetContextMenu() => null;
}