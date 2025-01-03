namespace DataCommander.Application;

internal interface IAsyncDataAdapter
{
    IResultWriter ResultWriter { get; }
    long RowCount { get; }
    int TableCount { get; }

    void Start();
    void Cancel();
}