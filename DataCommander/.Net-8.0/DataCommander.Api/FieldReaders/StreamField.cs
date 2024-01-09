using System.IO;

namespace DataCommander.Api.FieldReaders;

public sealed class StreamField(Stream stream)
{
    public Stream Stream { get; } = stream;
}