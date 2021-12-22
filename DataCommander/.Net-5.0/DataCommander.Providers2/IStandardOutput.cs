namespace DataCommander.Providers2;

public interface IStandardOutput
{
    void WriteLine(params object[] args);
    void Write(object arg);
}