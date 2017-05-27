namespace DataCommander.Providers.Field
{
    using System.IO;

    public sealed class StreamField
	{
        public StreamField( Stream stream )
		{
			Stream = stream;
		}

		public Stream Stream { get; }
	}
}
