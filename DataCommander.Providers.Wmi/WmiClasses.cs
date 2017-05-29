namespace DataCommander.Providers.Wmi
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Management;
    using System.Windows.Forms;

    internal sealed class WmiClasses : ITreeNode
    {
        private readonly ManagementScope scope;

        public WmiClasses(ManagementScope scope)
        {
            this.scope = scope;
        }

        public string Name => "Classes";

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var manClass = new ManagementClass(this.scope.Path);
            var objects = manClass.GetSubclasses();
            var list = new List<ITreeNode>();

            foreach (ManagementClass subClass in objects)
            {
                ITreeNode treeNode = new WmiClass(subClass);
                list.Add(treeNode);
            }

            return list.OrderBy(n => n.Name);
        }

        public bool Sortable => false;

        public string Query => null;

        public ContextMenuStrip ContextMenu => null;
    }
}