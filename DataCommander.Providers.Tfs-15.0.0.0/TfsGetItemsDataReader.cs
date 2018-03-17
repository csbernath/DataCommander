using Foundation.Data;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal sealed class TfsGetItemsDataReader : TfsDataReader
    {
        private readonly TfsCommand command;
        private bool first = true;
        private List<Item> items;
        private int index;

        public TfsGetItemsDataReader(TfsCommand command)
        {
            Assert.IsNotNull(command);
            this.command = command;
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

            if (command.Cancelled)
            {
                read = false;
            }
            else
            {
                if (first)
                {
                    first = false;
                    var parameters = command.Parameters;
                    var path = parameters["path"].GetValueOrDefault<string>();
                    var recursionString = Database.GetValueOrDefault<string>(parameters["recursion"].Value);
                    RecursionType recursion;

                    if (recursionString != null)
                    {
                        recursion = (RecursionType)Enum.Parse(typeof(RecursionType), recursionString);
                    }
                    else
                    {
                        recursion = RecursionType.OneLevel;
                    }

                    var itemSet = command.Connection.VersionControlServer.GetItems(path, recursion);
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

                    items = folders;
                    items.AddRange(files);
                }

                if (index < items.Count)
                {
                    var item = items[index];
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
                    index++;
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