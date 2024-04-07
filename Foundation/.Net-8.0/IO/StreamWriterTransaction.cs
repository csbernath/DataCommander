using System;
using System.ComponentModel;
using System.IO;
using System.Text;

namespace Foundation.IO;

public sealed class StreamWriterTransaction : IDisposable
{
    private readonly string _path;
    private readonly string _tempPath;
    private bool _commited;

    public StreamWriterTransaction(string path, string tempPath)
    {
        _path = path;
        _tempPath = tempPath;
        Writer = new StreamWriter(tempPath, false);
    }

    public StreamWriterTransaction(string path, string tempPath, Encoding encoding)
    {
        _path = path;
        _tempPath = tempPath;
        Writer = new StreamWriter(tempPath, false, encoding);
    }

    public StreamWriter Writer { get; }

    public void Commit()
    {
        Writer.Close();
        const NativeMethods.MoveFileExFlags flags = NativeMethods.MoveFileExFlags.ReplaceExisiting;
        var succeeded = NativeMethods.MoveFileEx(_tempPath, _path, flags);

        if (!succeeded)
            throw new Win32Exception();

        _commited = true;
    }

    void IDisposable.Dispose()
    {
        Writer.Dispose();

        if (!_commited)
            File.Delete(_tempPath);
    }
}