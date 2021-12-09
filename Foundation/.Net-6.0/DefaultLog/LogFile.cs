using System;
using System.IO;
using System.Text;
using Foundation.Core;
using Foundation.InternalLog;
using Foundation.Log;

namespace Foundation.DefaultLog
{
    internal sealed class LogFile : ILogFile
    {
        #region Private Fields

        private static readonly ILog Log = InternalLogFactory.Instance.GetTypeLog(typeof(LogFile));
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
            _path = path;
            _encoding = encoding;
            _bufferSize = bufferSize;
            _autoFlush = autoFlush;
            _formatter = formatter;
            _fileAttributes = fileAttributes;
            _dateTimeKind = dateTimeKind;
        }

        private FileStream Open(string fileName, DateTime dateTime)
        {
            _date = dateTime.Date;

            FileName = fileName.Replace("{date}", dateTime.ToString("yyyy.MM.dd"));
            FileName = FileName.Replace("{time}", dateTime.ToString("HH.mm.ss.fff"));
            FileName = FileName.Replace("{guid}", Guid.NewGuid().ToString());

            return new FileStream(FileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Read, _bufferSize);
        }

        private void Open(DateTime dateTime)
        {
            try
            {
                _fileStream = Open(_path, dateTime);
            }
            catch (Exception e)
            {
                Log.Write(LogLevel.Error, e.ToString());

                var directory = Path.GetTempPath();
                var fileName = Path.GetFileName(_path);
                _path = Path.Combine(directory, fileName);
                _fileStream = Open(_path, dateTime);

                Log.Write(LogLevel.Error, $"LogFile path: {FileName}");
            }

            if (_fileStream.Length == 0)
            {
                var preamble = _encoding.GetPreamble();
                _fileStream.Write(preamble, 0, preamble.Length);
            }
        }

        public void Write(DateTime dateTime, string text)
        {
            if (_fileStream == null)
                Open(dateTime);
            else if (dateTime.Date != _date)
            {
                Close();
                Open(dateTime);
            }

            var array = _encoding.GetBytes(text);
            _fileStream.Write(array, 0, array.Length);

            if (_autoFlush)
                _fileStream.Flush();
        }

        #region ILogFile Members

        public string FileName { get; private set; }

        void ILogFile.Open()
        {
            var begin = _formatter.Begin();

            if (begin != null)
                Write(LocalTime.Default.Now, begin);
        }

        void ILogFile.Write(LogEntry entry)
        {
            var text = _formatter.Format(entry);
            Write(entry.CreationTime, text);
        }

        void ILogFile.Flush() => _fileStream.Flush();

        public void Close()
        {
            if (_fileStream != null)
            {
                var end = _formatter.End();

                if (end != null)
                    Write(LocalTime.Default.Now, end);

                _fileStream.Close();
                var name = _fileStream.Name;
                _fileStream = null;

                if (_fileAttributes != default(FileAttributes))
                {
                    var attributes = File.GetAttributes(name);
                    attributes |= _fileAttributes; // FileAttributes.ReadOnly | FileAttributes.Hidden;
                    File.SetAttributes(name, attributes);
                }
            }
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
            Close();
        }

        #endregion
    }
}