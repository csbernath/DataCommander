using System.Collections.Generic;
using System.Data;
using System.Linq;
using Foundation.Assertions;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsObjectExplorer : IObjectExplorer
    {
        private TfsConnection _connection;
        private VersionControlServer _versionControlServer;

        public static string GetName(Item item)
        {
            Assert.IsTrue(item != null);

            var name = VersionControlPath.GetFileName(item.ServerItem);
            return name;
        }

#region IObjectExplorer Members

#region IObjectExplorer Members

        void IObjectExplorer.SetConnection(string connectionString, IDbConnection connection)
        {
            var tfsDbConnection = (TfsDbConnection)connection;
            this._connection = tfsDbConnection.Connection;
            var tfsTeamProjectCollection = this._connection.TfsTeamProjectCollection;
            _versionControlServer = (VersionControlServer)tfsTeamProjectCollection.GetService(typeof(VersionControlServer));
        }

#endregion

        IEnumerable<ITreeNode> IObjectExplorer.GetChildren(bool refresh)
        {
            // TFS 2005 and 2008 behavior is different:
            //  - TFS 2005 does not return the rootfolder
            //  - TFS 2008 returns the rootfolder
            var itemSet = _versionControlServer.GetItems(VersionControlPath.RootFolder, RecursionType.OneLevel);
            var items = itemSet.Items;
            IEnumerable<Item> enumerable;

            if (items.Length > 0)
            {
                var firstItem = items[0];

                if (firstItem.ServerItem == VersionControlPath.RootFolder)
                {
                    enumerable = items.Skip(1);
                }
                else
                {
                    enumerable = items;
                }
            }
            else
            {
                enumerable = items.Skip(1);
            }

            var f = from item in enumerable select (ITreeNode)ToTfsProject(item);
            return f;
        }

        bool IObjectExplorer.Sortable => false;

        private static TfsProject ToTfsProject(Item item)
        {
            return new TfsProject(item);
        }

#endregion
    }
}