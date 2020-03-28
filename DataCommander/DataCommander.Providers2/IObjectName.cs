namespace DataCommander.Providers2
{
    public interface IObjectName
    {
        string UnquotedName { get; }
        string QuotedName { get; }
    }
}