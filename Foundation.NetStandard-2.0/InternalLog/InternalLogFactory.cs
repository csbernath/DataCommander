using System;
using Foundation.Diagnostics;
using Foundation.Log;

namespace Foundation.InternalLog
{
    internal sealed class InternalLogFactory : ILogFactory
    {
        public static readonly InternalLogFactory Instance = new InternalLogFactory();

        private static TextLogWriter _textLogWriter;

        private InternalLogFactory() => _textLogWriter = new TextLogWriter(TraceWriter.Instance);

        void IDisposable.Dispose()
        {
        }

        string ILogFactory.FileName => null;

        ILog ILogFactory.GetLog(string name)
        {
            return new InternalLog(_textLogWriter, name);
        }
    }
}