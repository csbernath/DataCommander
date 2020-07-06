using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Foundation.Assertions;
using Foundation.Configuration;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsFile : ITreeNode
    {
        private readonly Item _item;

        public TfsFile(Item item)
        {
            Assert.IsTrue(item != null);
            _item = item;
        }

        #region ITreeNode Members

        string ITreeNode.Name => TfsObjectExplorer.GetName(_item);
        bool ITreeNode.IsLeaf => true;
        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh) => throw new NotImplementedException();
        bool ITreeNode.Sortable => false;
        string ITreeNode.Query => _item.ServerItem;

        ContextMenuStrip ITreeNode.ContextMenu
        {
            get
            {
                var contextMenu = new ContextMenuStrip();
                var items = contextMenu.Items;
                var menuItem = new ToolStripMenuItem("Open", null, Open_Click);
                items.Add(menuItem);

                var node = Settings.CurrentType;
                var attributes = node.Attributes;
                var name = attributes["Name"].GetValue<string>();

                menuItem = new ToolStripMenuItem(name, null, View_Click);
                items.Add(menuItem);
                return contextMenu;
            }
        }

        #endregion

        private void Open_Click(object sender, EventArgs e)
        {
            var name = VersionControlPath.GetFileName(_item.ServerItem);
            var localFileName = Path.GetTempPath();
            localFileName = Path.Combine(localFileName, name);
            _item.DownloadFile(localFileName);
            var startInfo = new ProcessStartInfo(localFileName);
            Process.Start(startInfo);
        }

        private void View_Click(object sender, EventArgs e)
        {
            var name = VersionControlPath.GetFileName(_item.ServerItem);
            var localFileName = Path.GetTempPath();
            localFileName = Path.Combine(localFileName, name);
            _item.DownloadFile(localFileName);

            var node = Settings.CurrentType;
            var attributes = node.Attributes;
            var contains = attributes.TryGetAttributeValue("FileName", out string fileName);
            var arguments = '"' + localFileName + '"';
            var startInfo = new ProcessStartInfo(fileName, arguments);
            Process.Start(startInfo);
        }
    }
}