﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;
using Foundation.Data;
using Foundation.Data.SqlClient;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class ExtendedStoreProcedureCollectionNode(DatabaseNode database) : ITreeNode
{
    string? ITreeNode.Name => "Extended Stored Procedures";
    bool ITreeNode.IsLeaf => false;
    bool ITreeNode.Sortable => false;
    string? ITreeNode.Query => null;
    public ContextMenu? GetContextMenu() => null;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken)
    {
        var executor = new SqlCommandExecutor(database.Databases.Server.CreateConnection);
        var commandText = @"select
    s.name,
    o.name
from sys.all_objects o
join sys.schemas s
    on o.schema_id = s.schema_id
where o.type = 'X'
order by 1,2";
        var request = new ExecuteReaderRequest(commandText);
        var childNodes = executor.ExecuteReader(request, 128, dataRecord =>
        {
            var schema = dataRecord.GetString(0);
            var name = dataRecord.GetString(1);
            return new ExtendedStoreProcedureNode(database, schema, name);
        });
        return Task.FromResult<IEnumerable<ITreeNode>>(childNodes);
    }
}