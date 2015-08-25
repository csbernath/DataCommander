namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public interface ISafeDbConnection
    {
        /// <summary>
        /// 
        /// </summary>
        CancellationToken CancellationToken { get; }

        /// <summary>
        /// 
        /// </summary>
        object Id { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="elapsed"></param>
        void HandleException(Exception exception, TimeSpan elapsed);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exception"></param>
        /// <param name="command"></param>
        void HandleException(Exception exception, IDbCommand command);
    }
}