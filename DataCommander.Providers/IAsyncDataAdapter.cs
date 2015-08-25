namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.Contracts;

    [ContractClass(typeof (IAsyncDataAdapterContract))]
    internal interface IAsyncDataAdapter
    {
        IResultWriter ResultWriter { get; }

        long RowCount { get; }

        int TableCount { get; }

        void BeginFill(
            IProvider provider,
            IEnumerable<IDbCommand> commands,
            int maxRecords,
            int rowBlockSize,
            IResultWriter resultWriter,
            Action<IAsyncDataAdapter, Exception> endFill,
            Action<IAsyncDataAdapter> writeEnd);

        void Cancel();
    }
}