using System;
using Foundation.Assertions;

namespace Foundation.Core
{
    public sealed class Disposer : IDisposable
    {
        private Action _dispose;
        private bool _disposed;

        public Disposer(Action dispose)
        {
            Assert.IsNotNull(dispose);
            _dispose = dispose;
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