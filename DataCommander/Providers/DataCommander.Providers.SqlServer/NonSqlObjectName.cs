using DataCommander.Api;

namespace DataCommander.Providers.SqlServer;

internal sealed class NonSqlObjectName(string objectName) : IObjectName
{
    string IObjectName.UnquotedName => objectName;
    string IObjectName.QuotedName => objectName;
}