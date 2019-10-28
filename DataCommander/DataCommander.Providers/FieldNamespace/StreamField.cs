using System.IO;

namespace DataCommander.Providers.FieldNamespace
{
    public sealed class StreamField
    {
        public StreamField(Stream stream)
        {
            Stream = stream;
        }

        public Stream Stream { get; }
    }
}