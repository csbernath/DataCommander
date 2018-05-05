using System;
using System.IO;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Log
{
    public sealed class TextLogWriter : ILogWriter
    {
        private readonly TextWriter _textWriter;
        private readonly ILogFormatter _formatter;

        public TextLogWriter(TextWriter textWriter, ILogFormatter logFormatter)
        {
            FoundationContract.Requires<ArgumentException>(textWriter != null);

            _textWriter = textWriter;
            _formatter = logFormatter;
        }

        void ILogWriter.Write(LogEntry entry)
        {
            var s = _formatter.Format(entry);
            _textWriter.Write(s);
        }

        #region ILogWriter Members

        void ILogWriter.Open()
        {
        }

        void ILogWriter.Flush() => _textWriter.Flush();
        void ILogWriter.Close() => _textWriter.Close();

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose() => _textWriter.Dispose();

        #endregion
    }
}