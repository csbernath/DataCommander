namespace DataCommander.Providers
{
    public interface IObjectName
    {
        string UnquotedName { get; }
        string QuotedName { get; }
    }
}