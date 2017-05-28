using System;
using System.Diagnostics;

namespace Foundation.Log
{
    /// <summary>
    /// 
    /// </summary>
    public class EventLogWriter : ILogWriter
    {
        private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof (EventLogWriter));
        private readonly EventLog _eventLog;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logName"></param>
        /// <param name="machineName"></param>
        /// <param name="source"></param>
        public EventLogWriter(
            string logName,
            string machineName,
            string source)
        {
            try
            {
                if (!EventLog.SourceExists(source, machineName))
                {
                    var sourceData = new EventSourceCreationData(source, logName) {MachineName = machineName};
                    EventLog.CreateEventSource(sourceData);
                }
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());
            }

            try
            {
                this._eventLog = new EventLog(logName, machineName, source);
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());
            }
        }

        void IDisposable.Dispose()
        {
            this._eventLog.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Open()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public void Flush()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            try
            {
                this._eventLog.Close();
            }
            catch
            {
            }
        }

        void ILogWriter.Write(LogEntry entry)
        {
            EventLogEntryType eventLogEntryType;

            switch (entry.LogLevel)
            {
                case LogLevel.Debug:
                case LogLevel.Trace:
                case LogLevel.Information:
                    eventLogEntryType = EventLogEntryType.Information;
                    break;

                case LogLevel.Warning:
                    eventLogEntryType = EventLogEntryType.Warning;
                    break;

                default:
                    eventLogEntryType = EventLogEntryType.Error;
                    break;
            }

            try
            {
                var message = TextLogFormatter.Format(entry);
                this._eventLog.WriteEntry(message, eventLogEntryType);
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());
            }
        }
    }
}