using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Foundation.Assertions;
using Foundation.Data;
using Microsoft.TeamFoundation.VersionControl.Client;

namespace DataCommander.Providers.Tfs
{
    internal sealed class TfsGetItemsDataReader : TfsDataReader
    {
        private readonly TfsCommand _command;
        private bool _first = true;
        private List<Item> _items;
        private int _index;

        public TfsGetItemsDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            this._command = command;
        }

        public override DataTable GetSchemaTable()
        {
            var table = CreateSchemaTable();
            AddSchemaRowString(table, "Name", false);
            AddSchemaRowInt32(table, "ChangesetId", false);
            AddSchemaRowDateTime(table, "CheckinDate", false);
            AddSchemaRowInt64(table, "ContentLength", false);
            AddSchemaRowString(table, "Encoding", false);
            AddSchemaRowString(table, "ItemType", false);
            return table;
        }

        public override bool Read()
        {
            bool read;

            if (_command.Cancelled)
                read = false;
            else
            {
                if (_first)
                {
                    _first = false;
                    var parameters = _command.Parameters;
                    var path = parameters["path"].GetValueOrDefault<string>();
                    var recursionString = ValueReader.GetValueOrDefault<string>(parameters["recursion"].Value);
                    RecursionType recursion;

                    if (recursionString != null)
                    {
                        recursion = (RecursionType) Enum.Parse(typeof(RecursionType), recursionString);
                    }
                    else
                    {
                        recursion = RecursionType.OneLevel;
                    }

                    var itemSet = _command.Connection.VersionControlServer.GetItems(path, recursion);
                    var folders = new List<Item>();
                    var files = new List<Item>();

                    foreach (var item in itemSet.Items.Skip(1))
                    {
                        switch (item.ItemType)
                        {
                            case ItemType.File:
                                files.Add(item);
                                break;

                            case ItemType.Folder:
                                folders.Add(item);
                                break;

                            default:
                                throw new NotImplementedException();
                        }
                    }

                    _items = folders;
                    _items.AddRange(files);
                }

                if (_index < _items.Count)
                {
                    var item = _items[_index];
                    var itemEncoding = item.Encoding;
                    string encodingString;

                    if (itemEncoding >= 0)
                    {
                        var encoding = Encoding.GetEncoding(item.Encoding);
                        encodingString = encoding.EncodingName;
                    }
                    else
                    {
                        encodingString = null;
                    }

                    var values = new object[]
                    {
                        item.ServerItem,
                        item.ChangesetId,
                        item.CheckinDate,
                        item.ContentLength,
                        encodingString,
                        item.ItemType.ToString()
                    };

                    Values = values;
                    read = true;
                    _index++;
                }
                else
                {
                    read = false;
                }
            }

            return read;
        }

        public override int RecordsAffected => -1;
        public override int FieldCount => 6;
    }
}