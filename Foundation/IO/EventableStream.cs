﻿using System;
using System.IO;

namespace Foundation.IO;

public class EventableStream : Stream
{
    private readonly Stream _stream;
    private EventHandler? _beforeRead;    

    public EventableStream(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);
        _stream = stream;
    }

    public override bool CanRead => _stream.CanRead;
    public override bool CanSeek => _stream.CanSeek;
    public override bool CanWrite => _stream.CanWrite;
    public override void Flush() => _stream.Flush();
    public override long Length => _stream.Length;

    public override long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public event EventHandler BeforeRead
    {
        add => _beforeRead += value;
        remove => _beforeRead -= value;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (_beforeRead != null)
            _beforeRead(this, EventArgs.Empty);

        return _stream.Read(buffer, offset, count);
    }

    public override long Seek(long offset, SeekOrigin origin) => _stream.Seek(offset, origin);
    public override void SetLength(long value) => _stream.SetLength(value);
    public override void Write(byte[] buffer, int offset, int count) => _stream.Write(buffer, offset, count);
}