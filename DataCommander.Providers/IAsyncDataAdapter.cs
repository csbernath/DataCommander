using DataCommander.Providers.ResultWriter;

namespace DataCommander.Providers
{
    //[ContractClass(typeof (IAsyncDataAdapterContract))]
    internal interface IAsyncDataAdapter
    {
        IResultWriter ResultWriter { get; }
        long RowCount { get; }
        int TableCount { get; }

        void Start();
        void Cancel();
    }
}