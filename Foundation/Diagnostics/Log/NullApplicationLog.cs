using System;

namespace Foundation.Diagnostics.Log
{
    internal sealed class NullApplicationLog : ILogFactory
    {
        private NullApplicationLog()
        {
        }

        public static NullApplicationLog Instance { get; } = new NullApplicationLog();

        #region IApplicationLog Members

        string ILogFactory.FileName => null;

        ILog ILogFactory.GetLog(string name)
        {
            return NullLog.Instance;
        }

        #endregion

        #region IDisposable Members

        void IDisposable.Dispose()
        {
        }

        #endregion
    }
}