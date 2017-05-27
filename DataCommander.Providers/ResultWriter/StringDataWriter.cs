namespace DataCommander.Providers.ResultWriter
{
    internal sealed class StringDataWriter : DataWriterBase
    {
        public override string ToString( object value )
        {
            return value.ToString().PadLeft(Width, ' ' );
        }
    }
}