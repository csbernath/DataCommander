using System;
using System.IO;
using System.Text;
using Foundation.Linq;

namespace Foundation.Diagnostics.Log
{
    /// <summary>
    /// <see cref="ILogWriter"/> implementation for logging a multithreaded application into a file.
    /// </summary>
    public class FileLogWriter : ILogWriter
    {
        private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof (FileLogWriter));
        private readonly bool _async;
        private readonly ILogFile _logFile;

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
            this._async = async;
            ILogFormatter formatter = new TextLogFormatter();
            // ILogFormatter formatter = new XmlLogFormatter();

            if (async)
            {
                this._logFile = new AsyncLogFile(path, encoding, bufferSize, timerPeriod, formatter, fileAttributes, dateTimeKind);
            }
            else
            {
                this._logFile = new LogFile(path, encoding, bufferSize, autoFlush, formatter, fileAttributes, dateTimeKind);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FileName => this._logFile.FileName;

        #region ILogWriter Members

        void ILogWriter.Open()
        {
            try
            {
                this._logFile.Open();
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        void ILogWriter.Write(LogEntry logEntry)
        {
            try
            {
                if (this._async)
                {
                    this._logFile.Write(logEntry);
                }
                else
                {
                    lock (this._logFile)
                    {
                        this._logFile.Write(logEntry);
                    }
                }
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());
            }
        }

        void ILogWriter.Flush()
        {
            try
            {
                this._logFile.Flush();
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        void ILogWriter.Close()
        {
            try
            {
                this._logFile.Close();
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToLogString());
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this._logFile.Dispose();
        }

        #endregion
    }
}