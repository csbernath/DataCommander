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
            Contract.Requires<ArgumentNullException>(dispose != null);
#endif

            this.dispose = dispose;
        }

        void IDisposable.Dispose()
        {
            if (!this.disposed && this.dispose != null)
            {
                this.dispose();
                this.disposed = true;
                this.dispose = null;
            }
        }
    }
}