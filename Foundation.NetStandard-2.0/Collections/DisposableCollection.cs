using System;
using System.Collections.ObjectModel;

namespace Foundation.Collections
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DisposableCollection<T> : Collection<T>, IDisposable where T : IDisposable
    {
        #region IDisposable Members

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            foreach (var item in this)
            {
                if (item != null)
                {
                    item.Dispose();
                }
            }
        }

        #endregion
    }
}