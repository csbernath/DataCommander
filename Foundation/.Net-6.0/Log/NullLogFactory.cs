using System;

namespace Foundation.Log
{
    public sealed class NullLogFactory : ILogFactory
    {
        public static readonly NullLogFactory Instance = new();

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