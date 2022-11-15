using System;

namespace DataCommander.Application.ResultWriter;

public sealed class BooleanDataWriter : DataWriterBase
{
    public override string ToString(object value)
    {
        string s;

        if (value == DBNull.Value)
            s = new string(' ', Width);
        else
            s = value.ToString().PadLeft(Width);

        return s;
    }
}