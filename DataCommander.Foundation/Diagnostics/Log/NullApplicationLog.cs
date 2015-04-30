namespace DataCommander.Foundation.Diagnostics
{
    using System;

    internal sealed class NullApplicationLog : ILogFactory
    {
        private static readonly NullApplicationLog instance = new NullApplicationLog();

        private NullApplicationLog()
        {
        }

        public static NullApplicationLog Instance
        {
            get
            {
                return instance;
            }
        }

        #region IApplicationLog Members

        string ILogFactory.FileName
        {
            get
            {
                return null;
            }
        }

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