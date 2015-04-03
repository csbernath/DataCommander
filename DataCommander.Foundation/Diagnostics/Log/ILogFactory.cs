namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public interface ILogFactory : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ILog GetLog(string name);
    }
}