namespace Binarit.Foundation.Threading
{
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public interface IWaitCallbackFactory
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        WaitCallback CreateWaitCallback();
    }
}