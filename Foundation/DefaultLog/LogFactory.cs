using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Foundation.Configuration;
using Foundation.Core;
using Foundation.Linq;
using Foundation.Log;

namespace Foundation.DefaultLog;

internal sealed class LogFactory : ILogFactory
{
    private readonly MultipleLog? _multipeLog;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LogFactory()
    {
        List<LogWriter> logWriters = [];
        var node = Settings.CurrentType!;

        node.Attributes.TryGetAttributeValue("DateTimeKind", DateTimeKind.Utc, out var dateTimeKind);
        _dateTimeProvider = dateTimeKind == DateTimeKind.Utc
            ? UniversalTime.Default
            : LocalTime.Default;

        var logWritersNode = node.ChildNodes["LogWriters"];

        foreach (var childNode in logWritersNode.ChildNodes)
        {
            var attributes = childNode.Attributes;
            var enabled = attributes["Enabled"].GetValue<bool>();

            if (enabled)
            {
                var logWriter = ReadLogWriter(childNode);
                if (logWriter != null)
                    logWriters.Add(logWriter);
            }
        }

        if (logWriters.Count > 0)
        {
            _multipeLog = new MultipleLog(logWriters);
            foreach (var logWriter in logWriters)
                logWriter.logWriter.Open();
        }
    }

    public LogFactory(bool forInternalUse)
    {
        var logWriter = new LogWriter(new TextLogWriter(TraceWriter.Instance, new TextLogFormatter()), LogLevel.Debug);
        _dateTimeProvider = LocalTime.Default;
        _multipeLog = new MultipleLog(logWriter.ItemToArray());
    }

    string? ILogFactory.FileName
    {
        get
        {
            var fileLogWriter = _multipeLog!.LogWriters.Select(w => w.logWriter).OfType<FileLogWriter>().FirstOrDefault();
            var fileName = fileLogWriter != null ? fileLogWriter.FileName : null;
            return fileName;
        }
    }

    ILog ILogFactory.GetLog(string? name) => new Log(this, name!);

    void ILogFactory.Write(IEnumerable<LogEntry> logEntries)
    {
        if (_multipeLog != null)
            foreach (var logEntry in logEntries)
                _multipeLog.Write(logEntry);
    }

    void IDisposable.Dispose()
    {
        if (_multipeLog != null)
            _multipeLog.Dispose();
    }

    internal void Write(Log log, LogLevel logLevel, string message)
    {
        if (_multipeLog != null)
        {
            var logEntry = LogEntryFactory.Create(log.LoggedName, _dateTimeProvider.Now, message, logLevel);
            _multipeLog.Write(logEntry);
        }
    }

    internal void Write(Log log, LogLevel logLevel, string format, params object[] args)
    {
        if (_multipeLog != null)
        {
            var message = string.Format(format, args);
            var logEntry = LogEntryFactory.Create(log.LoggedName, _dateTimeProvider.Now, message, logLevel);
            _multipeLog.Write(logEntry);
        }
    }

    internal void Write(Log log, LogLevel logLevel, Func<string> getMessage)
    {
        if (_multipeLog != null)
        {
            var message = getMessage();
            var logEntry = LogEntryFactory.Create(log.LoggedName, _dateTimeProvider.Now, message, logLevel);
            _multipeLog.Write(logEntry);
        }
    }

    private static LogWriter? ReadLogWriter(ConfigurationNode node)
    {
        LogWriter? logWriter = null;
        var attributes = node.Attributes;
        var type = attributes["Type"].GetValue<string>();
        var logLevel = attributes["LogLevel"].GetValue<LogLevel>();

        switch (type)
        {
            case "ConsoleLogWriter":
                logWriter = new LogWriter(ConsoleLogWriter.Instance, logLevel);
                break;

#if FOUNDATION_4_7
                case "EventLogWriter":
                {
                    var logName = attributes["LogName"].GetValue<string>();
                    var machineName = attributes["MachineName"].GetValue<string>();
                    var source = attributes["Source"].GetValue<string>();
                    var eventLogWriter = new EventLogWriter(logName, machineName, source);
                    logWriter = new LogWriter(eventLogWriter, logLevel);
                }

                    break;
#endif

            case "FileLogWriter":
            {
                var path = attributes["Path"].GetValue<string>()!;
                path = Environment.ExpandEnvironmentVariables(path);

                attributes.TryGetAttributeValue("Async", true, out var async);
                attributes.TryGetAttributeValue("BufferSize", 1048576, out var bufferSize); // 1 MB
                attributes.TryGetAttributeValue("TimerPeriod", TimeSpan.FromSeconds(10), out var timerPeriod);
                attributes.TryGetAttributeValue("AutoFlush", true, out var autoFlush);
                attributes.TryGetAttributeValue("FileAttributes", FileAttributes.ReadOnly | FileAttributes.Hidden, out var fileAttributes);
                attributes.TryGetAttributeValue("DateTimeKind", DateTimeKind.Utc, out var dateTimeKind);

                var fileLogWriter = new FileLogWriter(path, Encoding.UTF8, async, bufferSize, timerPeriod, autoFlush, fileAttributes, dateTimeKind);
                logWriter = new LogWriter(fileLogWriter, logLevel);
            }
                break;

            default:
                break;
        }

        return logWriter;
    }

    private sealed class LogWriter(ILogWriter logWriter, LogLevel logLevel)
    {
        public readonly ILogWriter logWriter = logWriter;
        public readonly LogLevel LogLevel = logLevel;
    }

    private sealed class MultipleLog : IDisposable
    {
        public MultipleLog(IEnumerable<LogWriter> logWriters)
        {
            ArgumentNullException.ThrowIfNull(logWriters);
            LogWriters = logWriters.ToArray();
        }

        public LogWriter[] LogWriters { get; }

        public void Dispose()
        {
            foreach (var logWriter in LogWriters)
            {
                logWriter.logWriter.Dispose();
            }
        }

        public void Write(LogEntry logEntry)
        {
            var logLevel = logEntry.LogLevel;
            for (var i = 0; i < LogWriters.Length; ++i)
            {
                var logWriter = LogWriters[i];
                if (logWriter.LogLevel >= logLevel)
                    logWriter.logWriter.Write(logEntry);
            }
        }
    }
}