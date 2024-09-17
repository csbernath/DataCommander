using System.Data;

namespace Foundation.Data.SqlClient;

/// <summary>
/// Summary description for LoggedSqlCommandFilterRule.
/// </summary>
internal sealed class SimpleLoggedSqlCommandFilterRule(
    bool include,
    string name,
    string hostName,
    string database,
    string text)
{
    public bool Match(
        string userName,
        string hostName1,
        IDbCommand command)
    {
        var database1 = command.Connection.Database;
        var commandText = command.CommandText;
        var match =
            (name == null || name == userName) &&
            (hostName == null || hostName == hostName1) &&
            (database == null || database == database1) &&
            (text == null || text == commandText);

        return match;
    }

    public bool Include { get; } = include;
}