namespace DataCommander.Api.ResultWriter;

public sealed class StringDataWriter : DataWriterBase
{
    public override string ToString(object value) => value.ToString().PadLeft(Width, ' ');
}