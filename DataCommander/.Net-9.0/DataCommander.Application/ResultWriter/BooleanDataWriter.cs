using System;

namespace DataCommander.Application.ResultWriter;

public sealed class BooleanDataWriter : DataWriterBase
{
    public override string ToString(object value) =>
        value == DBNull.Value
            ? new string(' ', Width)
            : value.ToString().PadLeft(Width);
}