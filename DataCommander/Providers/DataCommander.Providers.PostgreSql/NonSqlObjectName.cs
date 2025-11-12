using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql;

internal sealed class NonSqlObjectName(string objectName) : IObjectName
{
    string IObjectName.UnquotedName => objectName;

    string IObjectName.QuotedName => objectName;
}