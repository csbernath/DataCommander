namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;

    /// <summary>
    /// SQL logging can be filtered.
    /// </summary>
    public interface ISqlLoggedSqlCommandFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="hostName"></param>
        /// <param name="command"></param>
        /// <returns>True if the command must be logged. False otherwise.</returns>
        Boolean Contains(
            String userName,
            String hostName,
            IDbCommand command);
    }
}
