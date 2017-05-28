using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Foundation.Configuration;
using Foundation.Diagnostics;
using Foundation.Linq;

namespace Foundation.Log
{
    internal sealed class FoundationLogFactory : ILogFactory
    {
        private readonly MultipleLog _multipeLog;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FoundationLogFactory()
        {
            var logWriters = new List<LogWriter>();
            var node = Settings.CurrentType;
            
            var dateTimeKind = DateTimeKind.Utc;
            node.Attributes.TryGetAttributeValue("DateTimeKind", DateTimeKind.Utc, out dateTimeKind);
            this._dateTimeProvider = dateTimeKind == DateTimeKind.Utc
                ? (IDateTimeProvider) UniversalTime.Default
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
                    {
                        logWriter.LogLevel = attributes["LogLevel"].GetValue<LogLevel>();
                        logWriters.Add(logWriter);
                    }
                }
            }

            if (logWriters.Count > 0)
            {
                this._multipeLog = new MultipleLog(logWriters);
                LogFactory.Instance = this;

                foreach (var logWriter in logWriters)
                {
                    logWriter.logWriter.Open();
                }
            }
        }

        public FoundationLogFactory(bool forInternalUse)
        {
            var logWriter = new LogWriter
            {
                logWriter = new TextLogWriter(TraceWriter.Instance),
                LogLevel = LogLevel.Debug
            };

            this._dateTimeProvider = LocalTime.Default;
            this._multipeLog = new MultipleLog(logWriter.ItemAsEnumerable());
        }

        #region IApplicationLog Members

        string ILogFactory.FileName
        {
            get
            {
                string fileName;
                var fileLogWriter = this._multipeLog.LogWriters.Select(w => w.logWriter).OfType<FileLogWriter>().FirstOrDefault();

                if (fileLogWriter != null)
                {
                    fileName = fileLogWriter.FileName;
                }
                else
                {
                    fileName = null;
                }

                return fileName;
            }
        }

        ILog ILogFactory.GetLog(string name)
        {
            return new FoundationLog(this, name);
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this._multipeLog != null)
            {
                this._multipeLog.Dispose();
            }
        }

        #endregion

        internal void Write(FoundationLog log, LogLevel logLevel, string message)
        {
            if (this._multipeLog != null)
            {
                var logEntry = LogEntryFactory.Create(log.LoggedName, this._dateTimeProvider.Now, message, logLevel);
                this._multipeLog.Write(logEntry);
            }
        }

        internal void Write(FoundationLog log, LogLevel logLevel, string format, params object[] args)
        {
            if (this._multipeLog != null)
            {
                var message = string.Format(format, args);
                var logEntry = LogEntryFactory.Create(log.LoggedName, this._dateTimeProvider.Now, message, logLevel);
                this._multipeLog.Write(logEntry);
            }
        }

        internal void Write(FoundationLog log, LogLevel logLevel, Func<string> getMessage)
        {
            if (this._multipeLog != null)
            {
                var message = getMessage();
                var logEntry = LogEntryFactory.Create(log.LoggedName, this._dateTimeProvider.Now, message, logLevel);
                this._multipeLog.Write(logEntry);
            }
        }

        private static LogWriter ReadLogWriter(ConfigurationNode node)
        {
            LogWriter logWriter = null;
            var attributes = node.Attributes;
            var type = attributes["Type"].GetValue<string>();

            switch (type)
            {
                case "ConsoleLogWriter":
                    logWriter = new LogWriter
                    {
                        logWriter = ConsoleLogWriter.Instance
                    };
                    break;

                case "EventLogWriter":
                {
                    var logName = attributes["LogName"].GetValue<string>();
                    var machineName = attributes["MachineName"].GetValue<string>();
                    var source = attributes["Source"].GetValue<string>();
                    var eventLogWriter = new EventLogWriter(logName, machineName, source);
                    logWriter = new LogWriter
                    {
                        logWriter = eventLogWriter
                    };
                }

                    break;

                case "FileLogWriter":
                {
                    var path = attributes["Path"].GetValue<string>();
                    path = Environment.ExpandEnvironmentVariables(path);

                    var async = true;
                    attributes.TryGetAttributeValue("Async", async, out async);

                    var bufferSize = 1048576; // 1 MB
                    attributes.TryGetAttributeValue("BufferSize", bufferSize, out bufferSize);

                    var timerPeriod = TimeSpan.FromSeconds(10);
                    attributes.TryGetAttributeValue("TimerPeriod", timerPeriod, out timerPeriod);

                    var autoFlush = true;
                    attributes.TryGetAttributeValue("AutoFlush", autoFlush, out autoFlush);

                    FileAttributes fileAttributes;
                    attributes.TryGetAttributeValue("FileAttributes", FileAttributes.ReadOnly | FileAttributes.Hidden, out fileAttributes);

                    var dateTimeKind = DateTimeKind.Utc;
                    node.Attributes.TryGetAttributeValue("DateTimeKind", DateTimeKind.Utc, out dateTimeKind);

                    var fileLogWriter = new FileLogWriter(path, Encoding.UTF8, async, bufferSize, timerPeriod, autoFlush, fileAttributes, dateTimeKind);

                    logWriter = new LogWriter
                    {
                        logWriter = fileLogWriter
                    };
                }

                    break;

                default:
                    break;
            }

            return logWriter;
        }

        private sealed class LogWriter
        {
            public ILogWriter logWriter;
            public LogLevel LogLevel;
        }

        private sealed class MultipleLog : IDisposable
        {
            public MultipleLog(IEnumerable<LogWriter> logWriters)
            {
#if CONTRACTS_FULL
                Contract.Requires<ArgumentNullException>(logWriters != null);
#endif

                this.LogWriters = logWriters.ToArray();
            }

            public LogWriter[] LogWriters { get; }

#region IDisposable Members

            public void Dispose()
            {
                foreach (var logWriter in this.LogWriters)
                {
                    logWriter.logWriter.Dispose();
                }
            }

#endregion

            public void Write(LogEntry logEntry)
            {
                var logLevel = logEntry.LogLevel;
                for (var i = 0; i < this.LogWriters.Length; i++)
                {
                    var logWriter = this.LogWriters[i];
                    if (logWriter.LogLevel >= logLevel)
                    {
                        logWriter.logWriter.Write(logEntry);
                    }
                }
            }
        }
    }
}