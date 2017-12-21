using System;

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
#if CONTRACTS_FULL
            FoundationContract.Requires<ArgumentNullException>(dispose != null);
#endif

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