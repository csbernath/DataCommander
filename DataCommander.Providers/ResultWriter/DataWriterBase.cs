namespace DataCommander.Providers
{
    internal abstract class DataWriterBase
    {
        public void Init( int width )
        {
            this.Width = width;
        }

        public int Width { get; private set; }

        public abstract string ToString( object value );
    }
}