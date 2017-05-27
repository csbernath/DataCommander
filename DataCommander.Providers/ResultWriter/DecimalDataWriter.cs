namespace DataCommander.Providers.ResultWriter
{
    internal sealed class DecimalDataWriter : DataWriterBase
    {
        public override string ToString( object value )
        {
            return value.ToString().PadLeft(Width, ' ' );
        }
    }
}