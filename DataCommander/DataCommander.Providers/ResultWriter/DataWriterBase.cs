namespace DataCommander.Providers.ResultWriter
{
    internal abstract class DataWriterBase
    {
        public void Init( int width )
        {
            Width = width;
        }

        public int Width { get; private set; }

        public abstract string ToString( object value );
    }
}