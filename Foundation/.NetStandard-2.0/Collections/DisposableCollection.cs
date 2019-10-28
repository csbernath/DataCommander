using System;
using System.Collections.ObjectModel;

namespace Foundation.Collections
{
    public class DisposableCollection<T> : Collection<T>, IDisposable where T : IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            foreach (var item in this)
                if (item != null)
                    item.Dispose();
        }

        #endregion
    }
}