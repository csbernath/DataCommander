using System;

namespace DataCommander.Api;

[Flags]
public enum SqlObjectTypes
{
    Database = 0x01,
    Table = 0x02,
    View = 0x04,
    Procedure = 0x08,
    Function = 0x10,
    Column = 0x20,
    Trigger = 0x40,
    Index = 0x80,
    Value = 0x100
}