using System;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITransactionScope : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        void Complete();
    }
}