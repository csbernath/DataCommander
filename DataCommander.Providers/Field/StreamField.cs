namespace DataCommander.Providers
{
    using System.IO;

    public sealed class StreamField
	{
		private Stream stream;

		public StreamField( Stream stream )
		{
			this.stream = stream;
		}

		public Stream Stream
		{
			get
			{
				return this.stream;
			}
		}
	}
}
