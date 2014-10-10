namespace DataCommander.Providers
{
    internal abstract class DataWriterBase
    {
        private int width;

        public void Init( int width )
        {
            this.width = width;
        }

        public int Width
        {
            get
            {
                return this.width;
            }
        }

        public abstract string ToString( object value );
    }
}