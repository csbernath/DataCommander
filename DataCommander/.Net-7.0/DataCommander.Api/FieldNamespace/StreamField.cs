using System.IO;

namespace DataCommander.Api.FieldNamespace;

public sealed class StreamField
{
    public StreamField(Stream stream)
    {
        Stream = stream;
    }

    public Stream Stream { get; }
}