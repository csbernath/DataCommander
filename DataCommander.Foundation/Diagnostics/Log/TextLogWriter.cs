namespace DataCommander.Foundation.Diagnostics
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
        private readonly TextWriter textWriter;
        private readonly ILogFormatter formatter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="textWriter"></param>
        public TextLogWriter(TextWriter textWriter)
        {
#if CONTRACTS_FULL
            Contract.Requires(textWriter != null);
#endif

            this.textWriter = textWriter;
            this.formatter = new TextLogFormatter();
        }

        void ILogWriter.Write(LogEntry entry)
        {
            var s = this.formatter.Format(entry);
            this.textWriter.Write(s);
        }

#region ILogWriter Members

        void ILogWriter.Open()
        {
        }

        void ILogWriter.Flush()
        {
            this.textWriter.Flush();
        }

        void ILogWriter.Close()
        {
            this.textWriter.Close();
        }

#endregion

#region IDisposable Members

        void IDisposable.Dispose()
        {
            this.textWriter.Dispose();
        }

#endregion
    }
}