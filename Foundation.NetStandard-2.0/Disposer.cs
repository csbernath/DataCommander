using System;
using Foundation.Assertions;

namespace Foundation
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class Disposer : IDisposable
    {
        private Action _dispose;
        private bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dispose"></param>
        public Disposer(Action dispose)
        {
            Assert.IsNotNull(dispose);

            this._dispose = dispose;
        }

        void IDisposable.Dispose()
        {
            if (!_disposed && _dispose != null)
            {
                _dispose();
                _disposed = true;
                _dispose = null;
            }
        }
    }
}