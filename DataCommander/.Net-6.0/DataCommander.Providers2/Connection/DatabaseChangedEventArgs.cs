using System;

namespace DataCommander.Providers2.Connection;

public class DatabaseChangedEventArgs : EventArgs
{
    public readonly string Database;
    public DatabaseChangedEventArgs(string database) => Database = database;
}