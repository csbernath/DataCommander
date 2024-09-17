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
    private readonly MultipleLog _multipeLog;
    private readonly IDateTimeProvider _dateTimeProvider;

    public LogFactory()
    {
        List<LogWriter> logWriters = [];
        ConfigurationNode node = Settings.CurrentType;

        node.Attributes.TryGetAttributeValue("DateTimeKind", DateTimeKind.Utc, out DateTimeKind dateTimeKind);
        _dateTimeProvider = dateTimeKind == DateTimeKind.Utc
            ? UniversalTime.Default
            : LocalTime.Default;

        ConfigurationNode logWritersNode = node.ChildNodes["LogWriters"];

        foreach (ConfigurationNode childNode in logWritersNode.ChildNodes)
        {
            ConfigurationAttributeCollection attributes = childNode.Attributes;
            bool enabled = attributes["Enabled"].GetValue<bool>();

            if (enabled)
            {
                LogWriter logWriter = ReadLogWriter(childNode);
                if (logWriter != null)
                    logWriters.Add(logWriter);
            }
        }

        if (logWriters.Count > 0)
        {
            _multipeLog = new MultipleLog(logWriters);
            foreach (LogWriter logWriter in logWriters)
                logWriter.logWriter.Open();
        }
    }

    public LogFactory(bool forInternalUse)
    {
        LogWriter logWriter = new LogWriter(new TextLogWriter(TraceWriter.Instance, new TextLogFormatter()), LogLevel.Debug);
        _dateTimeProvider = LocalTime.Default;
        _multipeLog = new MultipleLog(logWriter.ItemToArray());
    }

    string ILogFactory.FileName
    {
        get
        {
            FileLogWriter fileLogWriter = _multipeLog.LogWriters.Select(w => w.logWriter).OfType<FileLogWriter>().FirstOrDefault();
            string fileName = fileLogWriter != null ? fileLogWriter.FileName : null;
            return fileName;
        }
    }

    ILog ILogFactory.GetLog(string name) => new Log(this, name);

    void IDisposable.Dispose()
    {
        if (_multipeLog != null)
            _multipeLog.Dispose();
    }

    internal void Write(Log log, LogLevel logLevel, string message)
    {
        if (_multipeLog != null)
        {
            LogEntry logEntry = LogEntryFactory.Create(log.LoggedName, _dateTimeProvider.Now, message, logLevel);
            _multipeLog.Write(logEntry);
        }
    }

    internal void Write(Log log, LogLevel logLevel, string format, params object[] args)
    {
        if (_multipeLog != null)
        {
            string message = string.Format(format, args);
            LogEntry logEntry = LogEntryFactory.Create(log.LoggedName, _dateTimeProvider.Now, message, logLevel);
            _multipeLog.Write(logEntry);
        }
    }

    internal void Write(Log log, LogLevel logLevel, Func<string> getMessage)
    {
        if (_multipeLog != null)
        {
            string message = getMessage();
            LogEntry logEntry = LogEntryFactory.Create(log.LoggedName, _dateTimeProvider.Now, message, logLevel);
            _multipeLog.Write(logEntry);
        }
    }

    private static LogWriter ReadLogWriter(ConfigurationNode node)
    {
        LogWriter logWriter = null;
        ConfigurationAttributeCollection attributes = node.Attributes;
        string type = attributes["Type"].GetValue<string>();
        LogLevel logLevel = attributes["LogLevel"].GetValue<LogLevel>();

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
                    string path = attributes["Path"].GetValue<string>();
                path = Environment.ExpandEnvironmentVariables(path);

                attributes.TryGetAttributeValue("Async", true, out bool async);
                attributes.TryGetAttributeValue("BufferSize", 1048576, out int bufferSize); // 1 MB
                attributes.TryGetAttributeValue("TimerPeriod", TimeSpan.FromSeconds(10), out TimeSpan timerPeriod);
                attributes.TryGetAttributeValue("AutoFlush", true, out bool autoFlush);
                attributes.TryGetAttributeValue("FileAttributes", FileAttributes.ReadOnly | FileAttributes.Hidden, out FileAttributes fileAttributes);
                attributes.TryGetAttributeValue("DateTimeKind", DateTimeKind.Utc, out DateTimeKind dateTimeKind);

                    FileLogWriter fileLogWriter = new FileLogWriter(path, Encoding.UTF8, async, bufferSize, timerPeriod, autoFlush, fileAttributes, dateTimeKind);
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
            foreach (LogWriter logWriter in LogWriters)
            {
                logWriter.logWriter.Dispose();
            }
        }

        public void Write(LogEntry logEntry)
        {
            LogLevel logLevel = logEntry.LogLevel;
            for (int i = 0; i < LogWriters.Length; ++i)
            {
                LogWriter logWriter = LogWriters[i];
                if (logWriter.LogLevel >= logLevel)
                    logWriter.logWriter.Write(logEntry);
            }
        }
    }
}