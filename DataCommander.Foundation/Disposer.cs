namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

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
            Contract.Requires<ArgumentNullException>(dispose != null);

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