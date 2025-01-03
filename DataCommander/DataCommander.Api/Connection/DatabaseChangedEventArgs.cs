using System;

namespace DataCommander.Api.Connection;

public class DatabaseChangedEventArgs : EventArgs
{
    public readonly string? Database;
    public DatabaseChangedEventArgs(string? database) => Database = database;
}