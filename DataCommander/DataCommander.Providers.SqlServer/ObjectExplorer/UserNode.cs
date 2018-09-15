using System.Collections.Generic;
using System.Windows.Forms;

namespace DataCommander.Providers.SqlServer.ObjectExplorer
{
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

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            return null;
        }

        public bool Sortable => false;

        public string Query
        {
            get
            {
                var query = string.Format(@"declare @uid smallint
select @uid = uid from {0}..sysusers where name = '{1}'

select u.name from {0}..sysmembers m
join {0}..sysusers u
on m.groupuid = u.uid
where memberuid = @uid
group by u.name", _database.Name, Name);

                return query;
            }
        }

        public ContextMenuStrip ContextMenu => null;
    }
}