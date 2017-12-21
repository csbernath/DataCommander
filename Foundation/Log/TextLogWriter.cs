using System;
using System.IO;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Log
{
    /// <summary>
    /// Writes log events to a <see cref="TextWriter"/>.
    /// </summary>
    /// <remarks>
    /// Use the following TextWriters for immediate logging your application:
    /// <list type="table">
    ///        <listheader>
    ///            <term>Name</term>
    ///            <description>Description</description>
    ///        </listheader>
    ///        <item>
    ///            <term><see cref="DebugWriter"/></term>
    ///            <description>Uses OutputDebugString.</description>    
    ///        </item>
    ///        <item>
    ///            <term><see cref="TraceWriter"/></term>
    ///            <description>Uses <see cref="System.Diagnostics.Trace.WriteLine(string)"/></description>
    ///        </item>
    ///    </list>
    /// </remarks>
    internal sealed class TextLogWriter : ILogWriter
    {
        private readonly TextWriter _textWriter;
        private readonly ILogFormatter _formatter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public TextLogWriter(TextWriter textWriter)
        {
            FoundationContract.Requires<ArgumentException>(textWriter != null);

            _textWriter = textWriter;
            _formatter = new TextLogFormatter();
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

        void ILogWriter.Flush()
        {
            _textWriter.Flush();
        }

        void ILogWriter.Close()
        {
            _textWriter.Close();
        }

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            _textWriter.Dispose();
        }

#endregion
    }
}