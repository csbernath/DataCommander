namespace DataCommander.Application.ResultWriter;

public sealed class DecimalDataWriter : DataWriterBase
{
    public override string ToString(object value) => value.ToString().PadLeft(Width, ' ');
}