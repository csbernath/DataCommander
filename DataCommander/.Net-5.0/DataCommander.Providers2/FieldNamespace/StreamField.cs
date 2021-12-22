using System.IO;

namespace DataCommander.Providers2.FieldNamespace;

public sealed class StreamField
{
    public StreamField(Stream stream)
    {
        Stream = stream;
    }

    public Stream Stream { get; }
}