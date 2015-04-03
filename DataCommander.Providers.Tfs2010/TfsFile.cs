namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Windows.Forms;
    using DataCommander.Foundation.Configuration;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    internal sealed class TfsFile : ITreeNode
    {
        private readonly Item item;

        public TfsFile(Item item)
        {
            Contract.Requires(item != null);
            this.item = item;
        }

        #region ITreeNode Members

        string ITreeNode.Name
        {
            get
            {
                return TfsObjectExplorer.GetName(this.item);
            }
        }

        bool ITreeNode.IsLeaf
        {
            get
            {
                return true;
            }
        }

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable
        {
            get
            {
                return false;
            }
        }

        string ITreeNode.Query
        {
            get
            {
                return this.item.ServerItem;
            }
        }

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                var contextMenu = new ContextMenuStrip();
                ToolStripItemCollection items = contextMenu.Items;
                var menuItem = new ToolStripMenuItem("Open", null, this.Open_Click);
                items.Add(menuItem);

                ConfigurationNode node = Settings.CurrentType;
                ConfigurationAttributeCollection attributes = node.Attributes;
                string name = attributes["Name"].GetValue<string>();

                menuItem = new ToolStripMenuItem(name, null, this.View_Click);
                items.Add(menuItem);
                return contextMenu;
            }
        }

        #endregion

        private void Open_Click(object sender, EventArgs e)
        {
            string name = VersionControlPath.GetFileName(this.item.ServerItem);
            string localFileName = Path.GetTempPath();
            localFileName = Path.Combine(localFileName, name);
            this.item.DownloadFile(localFileName);
            var startInfo = new ProcessStartInfo(localFileName);
            Process.Start(startInfo);
        }

        private void View_Click(object sender, EventArgs e)
        {
            string name = VersionControlPath.GetFileName(this.item.ServerItem);
            string localFileName = Path.GetTempPath();
            localFileName = Path.Combine(localFileName, name);
            this.item.DownloadFile(localFileName);

            ConfigurationNode node = Settings.CurrentType;
            ConfigurationAttributeCollection attributes = node.Attributes;
            string fileName;
            bool contains = attributes.TryGetAttributeValue("FileName", out fileName);
            string arguments = '"' + localFileName + '"';
            var startInfo = new ProcessStartInfo(fileName, arguments);
            Process.Start(startInfo);
        }
    }
}