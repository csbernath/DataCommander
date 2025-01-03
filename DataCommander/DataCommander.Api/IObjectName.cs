namespace DataCommander.Api;

public interface IObjectName
{
    string UnquotedName { get; }
    string QuotedName { get; }
}