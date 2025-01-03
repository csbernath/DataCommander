﻿using System;
using System.IO;
using System.Text;
using Foundation.Core;
using Foundation.InternalLog;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class LogFile(
    string path,
    Encoding encoding,
    int bufferSize,
    bool autoFlush,
    ILogFormatter formatter,
    FileAttributes fileAttributes,
    DateTimeKind dateTimeKind)
    : ILogFile
{
    private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof(LogFile));
    private DateTime _date;
    private FileStream? _fileStream;
    private string? _fileName;
    private readonly DateTimeKind _dateTimeKind = dateTimeKind;

    private FileStream Open(string fileName, DateTime dateTime)
    {
        _date = dateTime.Date;

        fileName = fileName.Replace("{date}", dateTime.ToString("yyyy.MM.dd"));
        fileName = fileName.Replace("{time}", dateTime.ToString("HH.mm.ss.fff"));
        fileName = fileName.Replace("{guid}", Guid.NewGuid().ToString());
        _fileName = fileName;

        return new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, bufferSize);
    }

    private void Open(DateTime dateTime)
    {
        try
        {
            _fileStream = Open(path, dateTime);
        }
        catch (Exception e)
        {
            Log.Write(LogLevel.Error, e.ToString());

            var directory = Path.GetTempPath();
            var fileName = Path.GetFileName(path);
            path = Path.Combine(directory, fileName);
            _fileStream = Open(path, dateTime);

            Log.Write(LogLevel.Error, $"LogFile path: {FileName}");
        }

        if (_fileStream.Length == 0)
        {
            var preamble = encoding.GetPreamble();
            _fileStream.Write(preamble, 0, preamble.Length);
        }
    }

    public void Write(DateTime dateTime, string text)
    {
        if (_fileStream == null)
            Open(dateTime);
        else if (dateTime.Date != _date)
        {
            Close();
            Open(dateTime);
        }

        var array = encoding.GetBytes(text);
        _fileStream!.Write(array, 0, array.Length);

        if (autoFlush)
            _fileStream.Flush();
    }

    public string? FileName => _fileName;

    void ILogFile.Open()
    {
        var begin = formatter.Begin();

        if (begin != null)
            Write(LocalTime.Default.Now, begin);
    }

    void ILogFile.Write(LogEntry entry)
    {
        var text = formatter.Format(entry);
        Write(entry.CreationTime, text);
    }

    void ILogFile.Flush() => _fileStream!.Flush();

    public void Close()
    {
        if (_fileStream != null)
        {
            var end = formatter.End();

            if (end != null)
                Write(LocalTime.Default.Now, end);

            _fileStream.Close();
            var name = _fileStream.Name;
            _fileStream = null;

            if (fileAttributes != default)
            {
                var attributes = File.GetAttributes(name);
                attributes |= fileAttributes; // FileAttributes.ReadOnly | FileAttributes.Hidden;
                File.SetAttributes(name, attributes);
            }
        }
    }

    void IDisposable.Dispose() => Close();
}