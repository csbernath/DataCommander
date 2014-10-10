namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Linq;
    using System.Text;
    using DataCommander.Foundation.Configuration;
    using DataCommander.Foundation.Linq;

    internal sealed class FoundationLogFactory : ILogFactory
    {
        private MultipleLog multipeLog;

        public FoundationLogFactory()
        {
            var logWriters = new List<LogWriter>();
            var node = Settings.CurrentType;
            var logWritersNode = node.ChildNodes[ "LogWriters" ];

            foreach (ConfigurationNode childNode in logWritersNode.ChildNodes)
            {
                var attributes = childNode.Attributes;
                Boolean enabled = attributes[ "Enabled" ].GetValue<Boolean>();

                if (enabled)
                {
                    var logWriter = ReadLogWriter( childNode );
                    if (logWriter != null)
                    {
                        logWriter.logLevel = attributes[ "LogLevel" ].GetValue<LogLevel>();
                        logWriters.Add( logWriter );
                    }
                }
            }

            if (logWriters.Count > 0)
            {
                this.multipeLog = new MultipleLog( logWriters );
                LogFactory.Instance = this;

                foreach (var logWriter in logWriters)
                {
                    logWriter.logWriter.Open();
                }
            }
        }

        public FoundationLogFactory( Boolean forInternalUse )
        {
            var logWriter = new LogWriter
            {
                logWriter = new TextLogWriter( TraceWriter.Instance ),
                logLevel = LogLevel.Debug
            };

            this.multipeLog = new MultipleLog( logWriter.ItemAsEnumerable() );
        }

        #region IApplicationLog Members

        ILog ILogFactory.GetLog( string name )
        {
            return new FoundationLog( this, name );
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            if (this.multipeLog != null)
            {
                this.multipeLog.Dispose();
            }
        }

        #endregion

        internal void Write( FoundationLog log, LogLevel logLevel, string message )
        {
            if (this.multipeLog != null)
            {
                var logEntry = LogEntryFactory.Create( log.LoggedName, message, logLevel );
                this.multipeLog.Write( logEntry );
            }
        }

        internal void Write( FoundationLog log, LogLevel logLevel, string format, params object[] args )
        {
            if (this.multipeLog != null)
            {
                String message = String.Format( format, args );
                var logEntry = LogEntryFactory.Create( log.LoggedName, message, logLevel );
                this.multipeLog.Write( logEntry );
            }
        }

        internal void Write( FoundationLog log, LogLevel logLevel, Func<String> getMessage )
        {
            if (this.multipeLog != null)
            {
                String message = getMessage();
                var logEntry = LogEntryFactory.Create( log.LoggedName, message, logLevel );
                this.multipeLog.Write( logEntry );
            }
        }

        private static LogWriter ReadLogWriter( ConfigurationNode node )
        {
            LogWriter logWriter = null;
            var attributes = node.Attributes;
            String type = attributes[ "Type" ].GetValue<String>();

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
                        String logName = attributes[ "LogName" ].GetValue<String>();
                        String machineName = attributes[ "MachineName" ].GetValue<String>();
                        String source = attributes[ "Source" ].GetValue<String>();
                        var eventLogWriter = new EventLogWriter( logName, machineName, source );
                        logWriter = new LogWriter
                        {
                            logWriter = eventLogWriter
                        };
                    }

                    break;

                case "FileLogWriter":
                    {
                        String path = attributes[ "Path" ].GetValue<String>();
                        path = Environment.ExpandEnvironmentVariables( path );

                        Boolean async = true;
                        attributes.TryGetAttributeValue<Boolean>( "Async", async, out async );
                        
                        Int32 queueCapacity = 100000; // 100.000 log entries
                        attributes.TryGetAttributeValue<Int32>( "QueueCapacity", queueCapacity, out queueCapacity );
                        
                        Int32 bufferSize = 1048576; // 1 MB
                        attributes.TryGetAttributeValue<Int32>( "BufferSize", bufferSize, out bufferSize );
                        
                        TimeSpan timerPeriod = TimeSpan.FromSeconds( 10 );
                        attributes.TryGetAttributeValue<TimeSpan>( "TimerPeriod", timerPeriod, out timerPeriod );

                        Boolean autoFlush = true;
                        attributes.TryGetAttributeValue<Boolean>( "AutoFlush", autoFlush, out autoFlush );

                        FileAttributes fileAttributes;
                        node.Attributes.TryGetAttributeValue<FileAttributes>( "FileAttributes", FileAttributes.ReadOnly | FileAttributes.Hidden, out fileAttributes );
                        var fileLogWriter = new FileLogWriter( path, Encoding.UTF8, async, queueCapacity, bufferSize, timerPeriod, autoFlush, fileAttributes );
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
            public LogLevel logLevel;
        }

        private sealed class MultipleLog : IDisposable
        {
            private LogWriter[] logWriters;

            public MultipleLog( IEnumerable<LogWriter> logWriters )
            {
                Contract.Requires( logWriters != null );

                this.logWriters = logWriters.ToArray();
            }

            #region IDisposable Members

            public void Dispose()
            {
                foreach (var logWriter in this.logWriters)
                {
                    logWriter.logWriter.Dispose();
                }
            }

            #endregion

            public void Write( LogEntry logEntry )
            {
                var logLevel = logEntry.LogLevel;
                for (Int32 i = 0; i < this.logWriters.Length; i++)
                {
                    var logWriter = this.logWriters[ i ];
                    if (logWriter.logLevel >= logLevel)
                    {
                        logWriter.logWriter.Write( logEntry );
                    }
                }
            }
        }
    }
}