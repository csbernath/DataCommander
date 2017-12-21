using Foundation.Diagnostics.Contracts;

namespace DataCommander.Providers
{
    using System;
    using System.Collections.Generic;
    using ResultWriter;

    //[ContractClassFor(typeof (IAsyncDataAdapter))]
    internal abstract class AsyncDataAdapterContract : IAsyncDataAdapter
    {
        IResultWriter IAsyncDataAdapter.ResultWriter => throw new NotImplementedException();

        long IAsyncDataAdapter.RowCount => throw new NotImplementedException();

        int IAsyncDataAdapter.TableCount => throw new NotImplementedException();

        void IAsyncDataAdapter.BeginFill(IProvider provider, IEnumerable<AsyncDataAdapterCommand> commands, int maxRecords, int rowBlockSize, IResultWriter resultWriter,
            Action<IAsyncDataAdapter, Exception> endFill, Action<IAsyncDataAdapter> writeEnd)
        {
            FoundationContract.Requires<ArgumentNullException>(provider != null);
            FoundationContract.Requires<ArgumentOutOfRangeException>(maxRecords >= 0);
            FoundationContract.Requires<ArgumentOutOfRangeException>(rowBlockSize >= 0);
            FoundationContract.Requires<ArgumentNullException>(resultWriter != null);
            FoundationContract.Requires<ArgumentNullException>(endFill != null);
            FoundationContract.Requires<ArgumentNullException>(writeEnd != null);
        }

        void IAsyncDataAdapter.Cancel()
        {
            throw new NotImplementedException();
        }
    }
}