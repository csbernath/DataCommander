using System.Threading;

namespace Foundation.Threading;

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