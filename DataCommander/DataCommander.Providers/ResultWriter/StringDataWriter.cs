namespace DataCommander.Providers.ResultWriter
{
    internal sealed class StringDataWriter : DataWriterBase
    {
        public override string ToString( object value ) => value.ToString().PadLeft(Width, ' ' );
    }
}