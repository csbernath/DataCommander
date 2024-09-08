using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class UserNode(DatabaseNode database, string? name) : ITreeNode
{
    public string? Name { get; } = name;
    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => null;

    public bool Sortable => false;

    public string Query
    {
        get
        {
            var query = $@"declare @uid smallint
select @uid = uid
from {database.Name}..sysusers
where name = '{Name}'

select u.name from {database.Name}..sysmembers m
join {database.Name}..sysusers u
    on m.groupuid = u.uid
where memberuid = @uid
group by u.name";

            return query;
        }
    }

    public ContextMenu? GetContextMenu() => null;
}