namespace DataCommander.Providers2;

public sealed class ObjectName : IObjectName
{
    private readonly string _objectName;

    public ObjectName(string objectName)
    {
        _objectName = objectName;
    }

    string IObjectName.UnquotedName => _objectName;

    string IObjectName.QuotedName => _objectName;
}