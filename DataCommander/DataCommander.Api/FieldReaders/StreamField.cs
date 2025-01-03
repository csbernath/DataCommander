using System.IO;

namespace DataCommander.Api.FieldReaders;

public sealed class StreamField
{
    public StreamField(Stream stream)
    {
        Stream = stream;
    }

    public Stream Stream { get; }
}