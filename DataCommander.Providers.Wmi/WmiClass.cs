namespace DataCommander.Providers.Wmi
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Management;
    using System.Windows.Forms;

    class WmiClass : ITreeNode
    {
        public WmiClass(ManagementClass manClass)
        {
            this.manClass = manClass;
        }

        public string Name => manClass.ClassPath.ClassName;

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            var objects = manClass.GetSubclasses();
            var list = new ArrayList();

            foreach (ManagementClass subClass in objects)
            {
                ITreeNode treeNode = new WmiClass(subClass);
                list.Add(treeNode);
            }

            var array = new ITreeNode[list.Count];
            list.CopyTo(array);

            return array;
        }
    
        public bool Sortable => false;

        public string Query => $"select * from {Name}";

        public ContextMenuStrip ContextMenu => null;

        readonly ManagementClass manClass;
    }
}