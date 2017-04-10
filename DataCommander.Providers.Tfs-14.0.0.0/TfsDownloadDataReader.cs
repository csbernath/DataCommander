namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.IO;
    using System.Linq;
    using DataCommander.Foundation;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Diagnostics;
    using Microsoft.TeamFoundation.VersionControl.Client;
    using Microsoft.TeamFoundation.VersionControl.Common;

    internal sealed class TfsDownloadDataReader : TfsDataReader
    {
        private static readonly ILog log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly TfsCommand command;
        private Item item;
        private string localPath;
        private bool first = true;
        private readonly Queue<Item> queue = new Queue<Item>();

        public TfsDownloadDataReader(TfsCommand command)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
#endif
            this.command = command;
        }

        public override int FieldCount => 4;

        public override int RecordsAffected => -1;

        public override DataTable GetSchemaTable()
        {
            var table = CreateSchemaTable();
            AddSchemaRowString(table, "ServerItem", false);
            AddSchemaRowInt32(table, "ItemType", false);
            AddSchemaRowDateTime(table, "CheckinDate", false);
            AddSchemaRowInt32(table, "ContentLength", false);
            return table;
        }

        public override bool Read()
        {
            bool read;

            if (this.command.Cancelled)
            {
                read = false;
            }
            else
            {
                if (this.first)
                {
                    this.first = false;
                    var serverPath = (string)this.command.Parameters["serverPath"].Value;
                    this.item = this.command.Connection.VersionControlServer.GetItem(serverPath);
                    this.localPath = Database.GetValueOrDefault<string>(this.command.Parameters["localPath"].Value);

                    if (this.localPath == null)
                    {
                        this.localPath = Path.GetTempPath();
                        this.localPath = Path.Combine(this.localPath, DateTime.Now.ToString("yyyyMMdd HHmmss.fff"));

                        switch (this.item.ItemType)
                        {
                            case ItemType.File:
                            case ItemType.Folder:
                                var name = VersionControlPath.GetFileName(this.item.ServerItem);
                                this.localPath = Path.Combine(this.localPath, name);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }

                    var queryForm = (QueryForm)DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                    queryForm.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, $"localPath: {this.localPath}"));


                    if (!VersionControlPath.IsValidPath(serverPath))
                    {
                        throw new ArgumentException($"The parameter serverPath '{serverPath}' is invalid.");
                    }

                    this.queue.Enqueue(this.item);
                }

                if (this.queue.Count > 0)
                {
                    var current = this.queue.Dequeue();
                    var values = new object[4];
                    values[0] = current.ServerItem;
                    values[1] = current.ItemType;
                    values[2] = current.CheckinDate;
                    values[3] = current.ContentLength;
                    this.Values = values;
                    var name = VersionControlPath.GetFileName(current.ServerItem);
                    string path;

                    if (this.item.ServerItem.Length + 1 <= current.ServerItem.Length)
                    {
                        var secondPath = current.ServerItem.Substring(this.item.ServerItem.Length + 1);
                        secondPath = secondPath.Replace(VersionControlPath.Separator, Path.DirectorySeparatorChar);
                        path = Path.Combine(this.localPath, secondPath);
                    }
                    else
                    {
                        path = this.localPath;
                    }

                    switch (current.ItemType)
                    {
                        case ItemType.File:
                            log.Write(LogLevel.Trace, "Downloading {0}...", current.ServerItem);
                            current.DownloadFile(path);
                            var checkingDate = current.CheckinDate;
                            var fileInfo = new FileInfo(path);
                            fileInfo.LastWriteTime = checkingDate;
                            fileInfo.Attributes = FileAttributes.ReadOnly;
                            break;

                        case ItemType.Folder:
                            if (!Directory.Exists(path))
                            {
                                var directoryInfo = Directory.CreateDirectory(path);

                                if (!directoryInfo.Exists)
                                {
                                    throw new ArgumentException($"The directory '{path}' does not exist.");
                                }
                            }

                            var itemSet = current.VersionControlServer.GetItems(current.ServerItem, RecursionType.OneLevel);

                            foreach (var childItem in itemSet.Items.Skip(1))
                            {
                                this.queue.Enqueue(childItem);
                            }

                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    read = true;
                }
                else
                {
                    read = false;
                }
            }

            return read;
        }
    }
}