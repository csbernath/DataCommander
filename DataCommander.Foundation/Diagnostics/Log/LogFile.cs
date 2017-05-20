namespace DataCommander.Foundation.Diagnostics.Log
{
    using System;
    using System.IO;
    using System.Text;

    internal sealed class LogFile : ILogFile
    {
        #region Private Fields

        private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof (LogFile));
        private string _path;
        private DateTime _date;
        private FileStream _fileStream;
        private readonly Encoding _encoding;
        private readonly int _bufferSize;
        private readonly bool _autoFlush;
        private readonly ILogFormatter _formatter;
        private readonly FileAttributes _fileAttributes;
        private readonly DateTimeKind _dateTimeKind;

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
            this._path = path;
            this._encoding = encoding;
            this._bufferSize = bufferSize;
            this._autoFlush = autoFlush;
            this._formatter = formatter;
            this._fileAttributes = fileAttributes;
            this._dateTimeKind = dateTimeKind;
        }

        private FileStream Open(string fileName, DateTime dateTime)
        {
            this._date = dateTime.Date;

            this.FileName = fileName.Replace("{date}", dateTime.ToString("yyyy.MM.dd"));
            this.FileName = this.FileName.Replace("{time}", dateTime.ToString("HH.mm.ss.fff"));
            this.FileName = this.FileName.Replace("{guid}", Guid.NewGuid().ToString());

            return new FileStream(this.FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, this._bufferSize);
        }

        private void Open(DateTime dateTime)
        {
            try
            {
                this._fileStream = this.Open(this._path, dateTime);
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());

                var directory = Path.GetTempPath();
                var fileName = Path.GetFileName(this._path);
                this._path = Path.Combine(directory, fileName);
                this._fileStream = this.Open(this._path, dateTime);

                Log.Write(LogLevel.Error, $"LogFile path: {this.FileName}");
            }

            if (this._fileStream.Length == 0)
            {
                var preamble = this._encoding.GetPreamble();
                this._fileStream.Write(preamble, 0, preamble.Length);
            }
        }

        public void Write(DateTime dateTime, string text)
        {
            if (this._fileStream == null)
            {
                this.Open(dateTime);
            }
            else if (dateTime.Date != this._date)
            {
                this.Close();
                this.Open(dateTime);
            }

            var array = this._encoding.GetBytes(text);
            this._fileStream.Write(array, 0, array.Length);

            if (this._autoFlush)
            {
                this._fileStream.Flush();
            }
        }

        #region ILogFile Members

        public string FileName { get; private set; }

        void ILogFile.Open()
        {
            var begin = this._formatter.Begin();

            if (begin != null)
            {
                this.Write(LocalTime.Default.Now, begin);
            }
        }

        void ILogFile.Write(LogEntry entry)
        {
            var text = this._formatter.Format(entry);
            this.Write(entry.CreationTime, text);
        }

        void ILogFile.Flush()
        {
            this._fileStream.Flush();
        }

        public void Close()
        {
            if (this._fileStream != null)
            {
                var end = this._formatter.End();

                if (end != null)
                {
                    this.Write(LocalTime.Default.Now, end);
                }

                this._fileStream.Close();
                var name = this._fileStream.Name;
                this._fileStream = null;

                if (this._fileAttributes != default(FileAttributes))
                {
                    var attributes = File.GetAttributes(name);
                    attributes |= this._fileAttributes; // FileAttributes.ReadOnly | FileAttributes.Hidden;
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