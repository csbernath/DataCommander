namespace DataCommander.Foundation.Diagnostics
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Text;

    internal sealed class LogFile : ILogFile
    {
        #region Private Fields

        private static readonly ILog log = InternalLogFactory.Instance.GetCurrentTypeLog();
        private string path;
        private string fileName;
        private DateTime date;
        private FileStream fileStream;
        private readonly Encoding encoding;
        private readonly int bufferSize;
        private readonly bool autoFlush;
        private readonly ILogFormatter formatter;
        private readonly FileAttributes fileAttributes;
        private readonly IDateTimeProvider dateTimeProvider;

        #endregion

        public LogFile(
            string path,
            Encoding encoding,
            int bufferSize,
            bool autoFlush,
            ILogFormatter formatter,
            FileAttributes fileAttributes,
            IDateTimeProvider dateTimeProvider)
        {
            Contract.Requires<ArgumentNullException>(dateTimeProvider != null);

            this.path = path;
            this.encoding = encoding;
            this.bufferSize = bufferSize;
            this.autoFlush = autoFlush;
            this.formatter = formatter;
            this.fileAttributes = fileAttributes;
            this.dateTimeProvider = dateTimeProvider;
        }

        private FileStream Open(string fileName)
        {
            DateTime now = dateTimeProvider.Now;
            this.date = now.Date;

            this.fileName = fileName.Replace("{date}", now.ToString("yyyy.MM.dd"));
            this.fileName = this.fileName.Replace("{time}", now.ToString("HH.mm.ss.fff"));
            this.fileName = this.fileName.Replace("{guid}", Guid.NewGuid().ToString());

            return new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, this.bufferSize);
        }

        private void Open()
        {
            try
            {
                this.fileStream = this.Open(this.path);
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToString());

                string directory = Path.GetTempPath();
                string fileName = Path.GetFileName(this.path);
                this.path = Path.Combine(directory, fileName);
                this.fileStream = this.Open(this.path);

                log.Write(LogLevel.Error, string.Format("LogFile path: {0}", this.fileName));
            }

            if (this.fileStream.Length == 0)
            {
                byte[] preamble = this.encoding.GetPreamble();
                this.fileStream.Write(preamble, 0, preamble.Length);
            }
        }

        public void Write(DateTime date, string text)
        {
            if (this.fileStream == null)
            {
                this.Open();
            }
            else if (date.Date != this.date)
            {
                this.Close();
                this.Open();
            }

            byte[] array = this.encoding.GetBytes(text);
            this.fileStream.Write(array, 0, array.Length);

            if (this.autoFlush)
            {
                this.fileStream.Flush();
            }
        }

        #region ILogFile Members

        public string FileName
        {
            get
            {
                return this.fileName;
            }
        }

        void ILogFile.Open()
        {
            string begin = this.formatter.Begin();

            if (begin != null)
            {
                this.Write(LocalTime.Default.Now, begin);
            }
        }

        void ILogFile.Write(LogEntry entry)
        {
            string text = this.formatter.Format(entry);
            this.Write(entry.CreationTime.Date, text);
        }

        void ILogFile.Flush()
        {
            this.fileStream.Flush();
        }

        public void Close()
        {
            if (this.fileStream != null)
            {
                string end = this.formatter.End();

                if (end != null)
                {
                    DateTime today = this.dateTimeProvider.Today();
                    this.Write(DateTime.Today, end);
                }

                this.fileStream.Close();
                string name = this.fileStream.Name;
                this.fileStream = null;

                if (this.fileAttributes != default(FileAttributes))
                {
                    FileAttributes attributes = File.GetAttributes(name);
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