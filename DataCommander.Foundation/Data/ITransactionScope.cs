namespace DataCommander.Foundation.Data
{
    using System;

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