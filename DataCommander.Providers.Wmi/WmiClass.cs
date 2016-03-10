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

        public string Name => this.manClass.ClassPath.ClassName;

        public bool IsLeaf => false;

        public IEnumerable<ITreeNode> GetChildren(bool refresh)
        {
            ManagementObjectCollection objects = this.manClass.GetSubclasses();
            ArrayList list = new ArrayList();

            foreach (ManagementClass subClass in objects)
            {
                ITreeNode treeNode = new WmiClass(subClass);
                list.Add(treeNode);
            }

            ITreeNode[] array = new ITreeNode[list.Count];
            list.CopyTo(array);

            return array;
        }
    
        public bool Sortable => false;

        public string Query => $"select * from {this.Name}";

        public ContextMenuStrip ContextMenu => null;

        readonly ManagementClass manClass;
    }
}