using System;
using Foundation.Diagnostics.Contracts;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Disposer : IDisposable
    {
        private Action dispose;
        private bool disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispose"></param>
        public Disposer(Action dispose)
        {
            FoundationContract.Requires<ArgumentNullException>(dispose != null);

            this.dispose = dispose;
        }

        void IDisposable.Dispose()
        {
            if (!disposed && dispose != null)
            {
                dispose();
                disposed = true;
                dispose = null;
            }
        }
    }
}