namespace DataCommander.Providers
{
    public interface IStandardOutput
    {
        void WriteLine(params object[] args);
        void Write(object arg);
    }
}