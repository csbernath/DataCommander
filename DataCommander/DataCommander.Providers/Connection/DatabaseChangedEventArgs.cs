using System;

namespace DataCommander.Providers.Connection
{
    public class DatabaseChangedEventArgs : EventArgs
    {
        public readonly string Database;
        public DatabaseChangedEventArgs(string database) => Database = database;
    }
}