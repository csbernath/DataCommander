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
        string database1 = command.Connection.Database;
        string commandText = command.CommandText;
        bool match =
            (name == null || name == userName) &&
            (hostName == null || hostName == hostName1) &&
            (database == null || database == database1) &&
            (text == null || text == commandText);

        return match;
    }

    public bool Include { get; } = include;
}