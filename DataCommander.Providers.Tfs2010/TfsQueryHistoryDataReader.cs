namespace DataCommander.Providers.Tfs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using DataCommander.Foundation.Data;
    using Microsoft.TeamFoundation.VersionControl.Client;

    internal class TfsQueryHistoryDataReader : TfsDataReader
    {
        #region Private Fields

        private readonly TfsCommand command;
        private bool first = true;
        private IEnumerator<Tuple<Changeset, int>> enumerator;
        private int recordCount;

        #endregion

        public TfsQueryHistoryDataReader(TfsCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            this.command = command;
        }

        public override DataTable GetSchemaTable()
        {
            DataTable table = CreateSchemaTable();
            AddSchemaRowInt32(table, "ChangesetId", false);
            AddSchemaRowString(table, "Committer", false);
            AddSchemaRowDateTime(table, "CreationDate", false);
            AddSchemaRowString(table, "Comment", false);
            AddSchemaRowString(table, "ChangeType", true);
            AddSchemaRowString(table, "ServerItem", true);
            return table;
        }

        public override bool Read()
        {
            bool read;

            if (this.first)
            {
                this.first = false;
                TfsParameterCollection parameters = this.command.Parameters;
                string path = (string)parameters["path"].Value;
                VersionSpec version = VersionSpec.Latest;
                int deletionId = 0;
                TfsParameter parameter = parameters.FirstOrDefault(p => p.ParameterName == "recursion");
                RecursionType recursion;
                if (parameter != null && parameter.Value != null && parameter.Value != DBNull.Value)
                {
                    recursion = (RecursionType)parameter.Value;
                }
                else
                {
                    recursion = RecursionType.Full;
                }

                string user = Database.GetValueOrDefault<string>(parameters["user"].Value);
                VersionSpec versionFrom = null;
                VersionSpec versionTo = null;
                parameter = parameters.FirstOrDefault( p => p.ParameterName == "maxCount" );
                int maxCount;
                if (parameter != null)
                {
                    maxCount = Database.GetValueOrDefault<int>( parameter.Value );
                    if (maxCount == 0)
                    {
                        maxCount = (int) parameter.DefaultValue;
                    }
                }
                else
                {
                    maxCount = (int)TfsDataReaderFactory.Dictionary[ this.command.CommandText ].Parameters[ "maxCount" ].DefaultValue;
                }

                bool includeChanges = Database.GetValueOrDefault<bool>(parameters["includeChanges"].Value);
                bool slotMode = Database.GetValueOrDefault<bool>(parameters["slotMode"].Value);
                IEnumerable changesets = this.command.Connection.VersionControlServer.QueryHistory(path, version, deletionId, recursion, user, versionFrom, versionTo, maxCount, includeChanges, slotMode);
                this.enumerator = this.AsEnumerable(changesets).GetEnumerator();
            }

            bool moveNext = this.enumerator.MoveNext();

            if (moveNext)
            {
                Tuple<Changeset, int> pair = this.enumerator.Current;
                Changeset changeset = pair.Item1;

                object[] values = new object[]
                {
                    changeset.ChangesetId,
                    changeset.Committer,
                    changeset.CreationDate,
                    changeset.Comment,
                    null,
                    null
                };

                int changeIndex = pair.Item2;

                if (changeIndex >= 0)
                {
                    Change change = changeset.Changes[changeIndex];
                    values[4] = change.ChangeType;
                    values[5] = change.Item.ServerItem;
                }

                this.Values = values;
                this.recordCount++;
                read = true;
            }
            else
            {
                read = false;
            }

            return read;
        }

        public override int RecordsAffected => -1;

        public override int FieldCount => 6;

        private IEnumerable<Tuple<Changeset, int>> AsEnumerable(IEnumerable changesets)
        {
            foreach (Changeset changeset in changesets)
            {
                Change[] changes = changeset.Changes;

                if (changes.Length > 0)
                {
                    for (int i = 0; i < changes.Length; i++)
                    {
                        yield return new Tuple<Changeset, int>(changeset, i);
                    }
                }
                else
                {
                    yield return new Tuple<Changeset, int>(changeset, -1);
                }
            }
        }
    }
}