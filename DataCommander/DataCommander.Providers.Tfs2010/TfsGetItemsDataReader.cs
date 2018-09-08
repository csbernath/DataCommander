namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Text;
    using Foundation.Data;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal sealed class TfsGetItemsDataReader : TfsDataReader
    {
        private readonly TfsCommand command;
        private bool first = true;
        private List<Item> items;
        private int index;

        public TfsGetItemsDataReader(TfsCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
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

            if (this.command.Cancelled)
            {
                read = false;
            }
            else
            {
                if (this.first)
                {
                    this.first = false;
                    var parameters = this.command.Parameters;
                    string path = parameters["path"].GetValueOrDefault<string>();
                    string recursionString = Database.GetValueOrDefault<string>(parameters["recursion"].Value);
                    RecursionType recursion;

                    if (recursionString != null)
                    {
                        recursion = (RecursionType)Enum.Parse(typeof(RecursionType), recursionString);
                    }
                    else
                    {
                        recursion = RecursionType.OneLevel;
                    }

                    ItemSet itemSet = this.command.Connection.VersionControlServer.GetItems(path, recursion);
                    List<Item> folders = new List<Item>();
                    List<Item> files = new List<Item>();

                    foreach (Item item in itemSet.Items.Skip(1))
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

                    this.items = folders;
                    this.items.AddRange(files);
                }

                if (this.index < this.items.Count)
                {
                    Item item = this.items[this.index];
                    int itemEncoding = item.Encoding;
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

                    this.Values = values;
                    read = true;
                    this.index++;
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