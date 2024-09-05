using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Foundation.IO;

public class AsyncTextWriter
{
    private readonly TextWriter _textWriter;
    private readonly List<AsyncTextWriterListItem> _list = [];
    private readonly object _syncObject = new();
    private readonly ManualResetEvent _waitHandle = new(false);
    private RegisteredWaitHandle _registeredWaitHandle;

    public AsyncTextWriter(TextWriter textWriter)
    {
        ArgumentNullException.ThrowIfNull(textWriter);
        _textWriter = textWriter;
    }

    private void Flush()
    {
        var sb = new StringBuilder();

        while (_list.Count > 0)
        {
            AsyncTextWriterListItem[] items;
            lock (_list)
            {
                var count = _list.Count;
                items = new AsyncTextWriterListItem[count];
                _list.CopyTo(items);
                _list.Clear();
            }

            for (var i = 0; i < items.Length; ++i)
                items[i].AppendTo(sb);
        }

        _textWriter.Write(sb);
        _textWriter.Flush();
    }

    private void Unregister()
    {
        lock (_syncObject)
        {
            if (_registeredWaitHandle != null)
            {
                ////log.Write(LogLevel.Trace,"Unregister...");
                var succeeded = _registeredWaitHandle.Unregister(null);
                _registeredWaitHandle = null;
                ////log.Write(LogLevel.Trace,"Unregister succeeded.");
            }
        }
    }

    private void Callback(object state, bool timedOut)
    {
        if (_list.Count > 0)
            Flush();
        else
            Unregister();
    }

    private void Write(AsyncTextWriterListItem item)
    {
        lock (_list)
            _list.Add(item);

        const int timeout = 10000; // 10 seconds

        lock (_syncObject)
        {
            if (_registeredWaitHandle == null)
            {
                // log.Write("ThreadPool.RegisterWaitForSingleObject",LogLevel.Trace);
                _registeredWaitHandle = ThreadPool.RegisterWaitForSingleObject(_waitHandle, Callback,
                    null, timeout, false);
            }
        }
    }

    public void Write(string value)
    {
        var item = new AsyncTextWriterListItem(DefaultFormatter.Instance, value);
        Write(item);
    }

    public void Write(IFormatter formatter, params object[] args)
    {
        var item = new AsyncTextWriterListItem(formatter, args);
        Write(item);
    }

    public void Close()
    {
        Unregister();
        Flush();
        _textWriter.Close();
    }
}