namespace DataCommander.Foundation.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Text;

    internal sealed class LogFile : ILogFile
    {
        #region Private Fields

        private static readonly ILog log = InternalLogFactory.Instance.GetTypeLog(typeof (LogFile));
        private string path;
        private DateTime date;
        private FileStream fileStream;
        private readonly Encoding encoding;
        private readonly int bufferSize;
        private readonly bool autoFlush;
        private readonly ILogFormatter formatter;
        private readonly FileAttributes fileAttributes;
        private readonly DateTimeKind dateTimeKind;

        #endregion

        public LogFile(
            string path,
            Encoding encoding,
            int bufferSize,
            bool autoFlush,
            ILogFormatter formatter,
            FileAttributes fileAttributes,
            DateTimeKind dateTimeKind)
        {
            this.path = path;
            this.encoding = encoding;
            this.bufferSize = bufferSize;
            this.autoFlush = autoFlush;
            this.formatter = formatter;
            this.fileAttributes = fileAttributes;
            this.dateTimeKind = dateTimeKind;
        }

        private FileStream Open(string fileName, DateTime dateTime)
        {
            this.date = dateTime.Date;

            this.FileName = fileName.Replace("{date}", dateTime.ToString("yyyy.MM.dd"));
            this.FileName = this.FileName.Replace("{time}", dateTime.ToString("HH.mm.ss.fff"));
            this.FileName = this.FileName.Replace("{guid}", Guid.NewGuid().ToString());

            return new FileStream(this.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, this.bufferSize);
        }

        private void Open(DateTime dateTime)
        {
            try
            {
                this.fileStream = this.Open(this.path, dateTime);
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToString());

                var directory = Path.GetTempPath();
                var fileName = Path.GetFileName(this.path);
                this.path = Path.Combine(directory, fileName);
                this.fileStream = this.Open(this.path, dateTime);

                log.Write(LogLevel.Error, $"LogFile path: {this.FileName}");
            }

            if (this.fileStream.Length == 0)
            {
                var preamble = this.encoding.GetPreamble();
                this.fileStream.Write(preamble, 0, preamble.Length);
            }
        }

        public void Write(DateTime dateTime, string text)
        {
            if (this.fileStream == null)
            {
                this.Open(dateTime);
            }
            else if (dateTime.Date != this.date)
            {
                this.Close();
                this.Open(dateTime);
            }

            var array = this.encoding.GetBytes(text);
            this.fileStream.Write(array, 0, array.Length);

            if (this.autoFlush)
            {
                this.fileStream.Flush();
            }
        }

        #region ILogFile Members

        public string FileName { get; private set; }

        void ILogFile.Open()
        {
            var begin = this.formatter.Begin();

            if (begin != null)
            {
                this.Write(LocalTime.Default.Now, begin);
            }
        }

        void ILogFile.Write(LogEntry entry)
        {
            var text = this.formatter.Format(entry);
            this.Write(entry.CreationTime, text);
        }

        void ILogFile.Flush()
        {
            this.fileStream.Flush();
        }

        public void Close()
        {
            if (this.fileStream != null)
            {
                var end = this.formatter.End();

                if (end != null)
                {
                    this.Write(LocalTime.Default.Now, end);
                }

                this.fileStream.Close();
                var name = this.fileStream.Name;
                this.fileStream = null;

                if (this.fileAttributes != default(FileAttributes))
                {
                    var attributes = File.GetAttributes(name);
                    attributes |= this.fileAttributes; // FileAttributes.ReadOnly | FileAttributes.Hidden;
                    File.SetAttributes(name, attributes);
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            this.Close();
        }

        #endregion
    }
}