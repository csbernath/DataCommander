using DataCommander.Api;

namespace DataCommander.Providers.PostgreSql;

internal sealed class NonSqlObjectName(string objectName) : IObjectName
{
    private readonly string _objectName = objectName;

    string IObjectName.UnquotedName => _objectName;

    string IObjectName.QuotedName => _objectName;
}