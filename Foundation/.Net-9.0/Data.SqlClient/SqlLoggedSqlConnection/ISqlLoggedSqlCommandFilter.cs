using System.Data;

namespace Foundation.Data.SqlClient.SqlLoggedSqlConnection;

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
    bool Contains(
        string userName,
        string hostName,
        IDbCommand command);
}