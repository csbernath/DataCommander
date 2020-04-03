using DataCommander.Providers2.ResultWriter;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class DecimalDataWriter : DataWriterBase
    {
        public override string ToString(object value) => value.ToString().PadLeft(Width, ' ');
    }
}