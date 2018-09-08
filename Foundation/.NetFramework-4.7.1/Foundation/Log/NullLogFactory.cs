using System;

namespace Foundation.Log
{
    internal sealed class NullLogFactory : ILogFactory
    {
        public static readonly NullLogFactory Instance = new NullLogFactory();

        private NullLogFactory()
        {
        }

        #region ILogFactory Members

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