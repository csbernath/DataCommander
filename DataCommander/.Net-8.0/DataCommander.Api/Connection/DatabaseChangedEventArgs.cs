using System;

namespace DataCommander.Api.Connection;

public class DatabaseChangedEventArgs(string? database) : EventArgs
{
    public readonly string? Database = database;
}