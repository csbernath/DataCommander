namespace DataCommander.Api;

public interface IStandardOutput
{
    void WriteLine(params object[] args);
    void Write(object arg);
}