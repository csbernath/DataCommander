using System;
using System.IO;

namespace Foundation.IO;

public sealed class TempFile : IDisposable
{
    private string _filename;
    private bool _deleted;

    public TempFile()
    {
    }

    public TempFile(string filename) => _filename = filename;

    public string Filename
    {
        get
        {
            if (_filename == null)
                _filename = Path.GetTempFileName();

            return _filename;
        }
    }

    public void Delete()
    {
        _deleted = true;
        File.Delete(_filename);
    }

    void IDisposable.Dispose()
    {
        if (_filename != null && !_deleted)
        {
            try
            {
                Delete();
            }
            catch
            {
            }
        }
    }
}