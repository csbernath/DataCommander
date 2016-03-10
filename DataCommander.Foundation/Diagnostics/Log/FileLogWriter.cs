namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.IO;
    using System.Text;
    using DataCommander.Foundation.Linq;

    /// <summary>
    /// <see cref="ILogWriter"/> implementation for logging a multithreaded application into a file.
    /// </summary>
    public class FileLogWriter : ILogWriter
    {
        private static readonly ILog log = InternalLogFactory.Instance.GetCurrentTypeLog();
        private readonly bool async;
        private readonly ILogFile logFile;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <param name="async"></param>
        /// <param name="bufferSize"></param>
        /// <param name="timerPeriod"></param>
        /// <param name="autoFlush"></param>
        /// <param name="fileAttributes"></param>
        /// <param name="dateTimeKind"></param>
        public FileLogWriter(
            string path,
            Encoding encoding,
            bool async,
            int bufferSize,
            TimeSpan timerPeriod,
            bool autoFlush,
            FileAttributes fileAttributes,
            DateTimeKind dateTimeKind)
        {
            this.async = async;
            ILogFormatter formatter = new TextLogFormatter();
            // ILogFormatter formatter = new XmlLogFormatter();

            if (async)
            {
                this.logFile = new AsyncLogFile(path, encoding, bufferSize, timerPeriod, formatter, fileAttributes, dateTimeKind);
            }
            else
            {
                this.logFile = new LogFile(path, encoding, bufferSize, autoFlush, formatter, fileAttributes, dateTimeKind);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FileName => this.logFile.FileName;

        #region ILogWriter Members

        void ILogWriter.Open()
        {
            try
            {
                this.logFile.Open();
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        void ILogWriter.Write(LogEntry logEntry)
        {
            try
            {
                if (this.async)
                {
                    this.logFile.Write(logEntry);
                }
                else
                {
                    lock (this.logFile)
                    {
                        this.logFile.Write(logEntry);
                    }
                }
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToString());
            }
        }

        void ILogWriter.Flush()
        {
            try
            {
                this.logFile.Flush();
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        void ILogWriter.Close()
        {
            try
            {
                this.logFile.Close();
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.logFile.Dispose();
        }

        #endregion
    }
}