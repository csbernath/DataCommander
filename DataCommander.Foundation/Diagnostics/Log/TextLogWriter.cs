namespace DataCommander.Foundation.Diagnostics.Log
{
    using System;
    using System.IO;

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
#if CONTRACTS_FULL
            Contract.Requires(textWriter != null);
#endif

            this._textWriter = textWriter;
            this._formatter = new TextLogFormatter();
        }

        void ILogWriter.Write(LogEntry entry)
        {
            var s = this._formatter.Format(entry);
            this._textWriter.Write(s);
        }

#region ILogWriter Members

        void ILogWriter.Open()
        {
        }

        void ILogWriter.Flush()
        {
            this._textWriter.Flush();
        }

        void ILogWriter.Close()
        {
            this._textWriter.Close();
        }

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            this._textWriter.Dispose();
        }

#endregion
    }
}