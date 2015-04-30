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

        #endregion

        public LogFile(
            string path,
            Encoding encoding,
            int bufferSize,
            bool autoFlush,
            ILogFormatter formatter,
            FileAttributes fileAttributes)
        {
            this.path = path;
            this.encoding = encoding;
            this.bufferSize = bufferSize;
            this.autoFlush = autoFlush;
            this.formatter = formatter;
            this.fileAttributes = fileAttributes;
        }

        private static string GetNextFileName(string path, out DateTime date)
        {
            date = DateTime.Today;
            var sb = new StringBuilder();
            string directoryName = Path.GetDirectoryName(path);

            if (directoryName.Length > 0)
            {
                sb.Append(directoryName);
                sb.Append(Path.DirectorySeparatorChar);
            }

            sb.Append(Path.GetFileNameWithoutExtension(path));
            sb.Append(' ');
            sb.Append(date.ToString("yyyy.MM.dd"));
            sb.Append(" [{0}]");
            sb.Append(Path.GetExtension(path));
            string format = sb.ToString();
            int id = 0;
            string idStr;

            while (true)
            {
                idStr = id.ToString().PadLeft(2, '0');
                string fileName = string.Format(format, idStr);
                bool exists = File.Exists(fileName);

                if (!exists)
                {
                    break;
                }

                id++;
            }

            return string.Format(format, idStr);
        }

        private void Open()
        {
            this.fileName = GetNextFileName(this.path, out this.date);

            try
            {
                this.fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read,
                    this.bufferSize);
            }
            catch (Exception e)
            {
                log.Write(LogLevel.Error, e.ToString());
                string directory = Path.GetTempPath();
                this.fileName = Path.GetFileName(this.path);
                StringBuilder sb = new StringBuilder();
                sb.Append(directory);
                sb.Append(this.fileName);
                this.path = sb.ToString();
                this.fileName = GetNextFileName(this.path, out this.date);
                this.fileStream = new FileStream(this.fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read,
                    this.bufferSize, true);
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
            else if (date != this.date)
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