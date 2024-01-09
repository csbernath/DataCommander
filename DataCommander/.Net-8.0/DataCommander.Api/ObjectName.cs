namespace DataCommander.Api;

public sealed class ObjectName(string objectName) : IObjectName
{
    string IObjectName.UnquotedName => objectName;

    string IObjectName.QuotedName => objectName;
}