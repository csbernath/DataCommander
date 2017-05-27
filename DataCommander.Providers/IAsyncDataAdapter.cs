using System;
using System.Collections.Generic;
using DataCommander.Providers.ResultWriter;

namespace DataCommander.Providers
{
    //[ContractClass(typeof (IAsyncDataAdapterContract))]
    internal interface IAsyncDataAdapter
    {
        IResultWriter ResultWriter { get; }
        long RowCount { get; }
        int TableCount { get; }

        void BeginFill(
            IProvider provider,
            IEnumerable<AsyncDataAdapterCommand> commands,
            int maxRecords,
            int rowBlockSize,
            IResultWriter resultWriter,
            Action<IAsyncDataAdapter, Exception> endFill,
            Action<IAsyncDataAdapter> writeEnd);

        void Cancel();
    }
}