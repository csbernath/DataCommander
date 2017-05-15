namespace DataCommander.Providers.Field
{
    using System.IO;

    public sealed class StreamField
	{
        public StreamField( Stream stream )
		{
			this.Stream = stream;
		}

		public Stream Stream { get; }
	}
}
