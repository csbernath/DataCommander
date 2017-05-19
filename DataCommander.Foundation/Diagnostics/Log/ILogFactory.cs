namespace DataCommander.Foundation.Diagnostics.Log
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
        string FileName { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        ILog GetLog(string name);
    }
}