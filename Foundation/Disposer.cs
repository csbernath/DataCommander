using System;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;

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
            Assert.IsNotNull(dispose);

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