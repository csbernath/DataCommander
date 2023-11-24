using System.Data;

namespace Foundation.Data.SqlClient;

/// <summary>
/// Summary description for LoggedSqlCommandFilterRule.
/// </summary>
internal sealed class SimpleLoggedSqlCommandFilterRule
{
    private readonly string _userName;
    private readonly string _hostName;
    private readonly string _database;
    private readonly string _commandText;

    public SimpleLoggedSqlCommandFilterRule(
        bool include,
        string userName,
        string hostName,
        string database,
        string commandText)
    {
        Include = include;
        _userName = userName;
        _hostName = hostName;
        _database = database;
        _commandText = commandText;
    }

    public bool Match(
        string userName,
        string hostName,
        IDbCommand command)
    {
        var database = command.Connection.Database;
        var commandText = command.CommandText;
        var match =
            (_userName == null || _userName == userName) &&
            (_hostName == null || _hostName == hostName) &&
            (_database == null || _database == database) &&
            (_commandText == null || _commandText == commandText);

        return match;
    }

    public bool Include { get; }
}