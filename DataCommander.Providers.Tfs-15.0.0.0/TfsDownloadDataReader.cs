using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using DataCommander.Providers.Connection;
using DataCommander.Providers.Query;
using Foundation;
using Foundation.Assertions;
using Foundation.Data;
using Foundation.Log;
using Microsoft.TeamFoundation.VersionControl.Client;
using Microsoft.TeamFoundation.VersionControl.Common;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsDownloadDataReader : TfsDataReader
    {
        private static readonly ILog Log = LogFactory.Instance.GetCurrentTypeLog();
        private readonly TfsCommand _command;
        private Item _item;
        private string _localPath;
        private bool _first = true;
        private readonly Queue<Item> _queue = new Queue<Item>();

        public TfsDownloadDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            this._command = command;
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

            if (_command.Cancelled)
            {
                read = false;
            }
            else
            {
                if (_first)
                {
                    _first = false;
                    var serverPath = (string) _command.Parameters["serverPath"].Value;
                    _item = _command.Connection.VersionControlServer.GetItem(serverPath);
                    _localPath = ValueReader.GetValueOrDefault<string>(_command.Parameters["localPath"].Value);

                    if (_localPath == null)
                    {
                        _localPath = Path.GetTempPath();
                        _localPath = Path.Combine(_localPath, DateTime.Now.ToString("yyyyMMdd HHmmss.fff"));

                        switch (_item.ItemType)
                        {
                            case ItemType.File:
                            case ItemType.Folder:
                                var name = VersionControlPath.GetFileName(_item.ServerItem);
                                _localPath = Path.Combine(_localPath, name);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }

                    var queryForm = (QueryForm) DataCommanderApplication.Instance.MainForm.ActiveMdiChild;
                    queryForm.AddInfoMessage(new InfoMessage(LocalTime.Default.Now, InfoMessageSeverity.Information, null, $"localPath: {_localPath}"));

                    if (!VersionControlPath.IsValidPath(serverPath))
                        throw new ArgumentException($"The parameter serverPath '{serverPath}' is invalid.");

                    _queue.Enqueue(_item);
                }

                if (_queue.Count > 0)
                {
                    var current = _queue.Dequeue();
                    var values = new object[4];
                    values[0] = current.ServerItem;
                    values[1] = current.ItemType;
                    values[2] = current.CheckinDate;
                    values[3] = current.ContentLength;
                    Values = values;
                    var name = VersionControlPath.GetFileName(current.ServerItem);
                    string path;

                    if (_item.ServerItem.Length + 1 <= current.ServerItem.Length)
                    {
                        var secondPath = current.ServerItem.Substring(_item.ServerItem.Length + 1);
                        secondPath = secondPath.Replace(VersionControlPath.Separator, Path.DirectorySeparatorChar);
                        path = Path.Combine(_localPath, secondPath);
                    }
                    else
                    {
                        path = _localPath;
                    }

                    switch (current.ItemType)
                    {
                        case ItemType.File:
                            Log.Write(LogLevel.Trace, "Downloading {0}...", current.ServerItem);
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
                                _queue.Enqueue(childItem);
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