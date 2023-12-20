using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class RoleNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public RoleNode(DatabaseNode database, string name)
    {
        _database = database;
        Name = name;
    }

    public string Name { get; }
    public bool IsLeaf => true;

    Task<IEnumerable<ITreeNode>> ITreeNode.GetChildren(bool refresh, CancellationToken cancellationToken) => null;
    public bool Sortable => false;

    public string Query
    {
        get
        {
            var query = string.Format(@"declare @uid smallint
select @uid = uid from {0}..sysusers where name = '{1}'

select u.name from {0}..sysmembers m
join {0}..sysusers u
on m.memberuid = u.uid
where m.groupuid = @uid
order by u.name", _database.Name, Name);

            return query;
        }
    }

    public ContextMenu? GetContextMenu() => null;
}