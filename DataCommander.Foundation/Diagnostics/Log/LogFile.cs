namespace DataCommander.Foundation.Diagnostics
{
    using System;
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

            this.fileName = fileName.Replace("{date}", dateTime.ToString("yyyy.MM.dd"));
            this.fileName = this.fileName.Replace("{time}", dateTime.ToString("HH.mm.ss.fff"));
            this.fileName = this.fileName.Replace("{guid}", Guid.NewGuid().ToString());

            return new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, this.bufferSize);
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

                string directory = Path.GetTempPath();
                string fileName = Path.GetFileName(this.path);
                this.path = Path.Combine(directory, fileName);
                this.fileStream = this.Open(this.path, dateTime);

                log.Write(LogLevel.Error, $"LogFile path: {this.fileName}");
            }

            if (this.fileStream.Length == 0)
            {
                byte[] preamble = this.encoding.GetPreamble();
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
                string end = this.formatter.End();

                if (end != null)
                {
                    this.Write(LocalTime.Default.Now, end);
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