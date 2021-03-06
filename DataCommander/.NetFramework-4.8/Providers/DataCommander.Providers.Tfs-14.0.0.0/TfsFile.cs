﻿using Foundation.Configuration;
using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    internal sealed class TfsFile : ITreeNode
    {
        private readonly Item item;

        public TfsFile(Item item)
        {
            FoundationContract.Requires<ArgumentException>(item != null);

            this.item = item;
        }

#region ITreeNode Members

        string ITreeNode.Name => TfsObjectExplorer.GetName(item);

        bool ITreeNode.IsLeaf => true;

        IEnumerable<ITreeNode> ITreeNode.GetChildren(bool refresh)
        {
            throw new NotImplementedException();
        }

        bool ITreeNode.Sortable => false;

        string ITreeNode.Query => item.ServerItem;

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
            var name = VersionControlPath.GetFileName(item.ServerItem);
            var localFileName = Path.GetTempPath();
            localFileName = Path.Combine(localFileName, name);
            item.DownloadFile(localFileName);
            var startInfo = new ProcessStartInfo(localFileName);
            Process.Start(startInfo);
        }

        private void View_Click(object sender, EventArgs e)
        {
            var name = VersionControlPath.GetFileName(item.ServerItem);
            var localFileName = Path.GetTempPath();
            localFileName = Path.Combine(localFileName, name);
            item.DownloadFile(localFileName);

            var node = Settings.CurrentType;
            var attributes = node.Attributes;
            string fileName;
            var contains = attributes.TryGetAttributeValue("FileName", out fileName);
            var arguments = '"' + localFileName + '"';
            var startInfo = new ProcessStartInfo(fileName, arguments);
            Process.Start(startInfo);
        }
    }
}