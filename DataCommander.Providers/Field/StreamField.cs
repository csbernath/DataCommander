namespace DataCommander.Providers
{
    using System.IO;

    public sealed class StreamField
	{
		private readonly Stream stream;

		public StreamField( Stream stream )
		{
			this.stream = stream;
		}

		public Stream Stream => this.stream;
	}
}
