using System.Collections.Generic;
using DataCommander.Api;

namespace DataCommander.Providers.SqlServer.ObjectExplorer;

internal sealed class UserNode : ITreeNode
{
    private readonly DatabaseNode _database;

    public UserNode(DatabaseNode database, string name)
    {
        _database = database;
        Name = name;
    }

    public string Name { get; }
    public bool IsLeaf => true;

    IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => null;

    public bool Sortable => false;

    public string Query
    {
        get
        {
            var query = $@"declare @uid smallint
select @uid = uid
from {_database.Name}..sysusers
where name = '{Name}'

select u.name from {_database.Name}..sysmembers m
join {_database.Name}..sysusers u
    on m.groupuid = u.uid
where memberuid = @uid
group by u.name";

            return query;
        }
    }

    public ContextMenu GetContextMenu()
    {
        throw new System.NotImplementedException();
    }
}