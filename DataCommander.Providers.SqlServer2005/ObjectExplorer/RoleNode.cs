namespace DataCommander.Providers.SqlServer2005.ObjectExplorer
{
    using System.Collections.Generic;
    using System.Windows.Forms;

    internal sealed class RoleNode : ITreeNode
    {
        private readonly DatabaseNode database;

        public RoleNode(DatabaseNode database, string name)
        {
            this.database = database;
            this.Name = name;
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
on m.memberuid = u.uid
where m.groupuid = @uid
order by u.name", this.database.Name, this.Name);

                return query;
            }
        }

        public ContextMenuStrip ContextMenu => null;
    }
}