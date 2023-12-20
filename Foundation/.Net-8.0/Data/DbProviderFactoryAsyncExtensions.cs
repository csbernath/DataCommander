using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data;

public static class DbProviderFactoryAsyncExtensions
{
    public static async Task<ReadOnlySegmentLinkedList<T>> ExecuteReaderAsync<T>(
        this DbProviderFactory dbProviderFactory,
        string connectionString,
        ExecuteReaderRequest request,
        int segmentLength,
        Func<IDataRecord, T> readRecord,
        CancellationToken cancellationToken)
    {
        ReadOnlySegmentLinkedList<T> rows = null;
        await dbProviderFactory.ExecuteReaderAsync(
            connectionString,
            request,
            async dataReader => rows = await dataReader.ReadResultAsync(segmentLength, readRecord, cancellationToken));
        return rows;
    }

    public static async Task ExecuteReaderAsync(
        this DbProviderFactory dbProviderFactory,
        string connectionString,
        ExecuteReaderRequest request,
        Func<DbDataReader, Task> readResults)
    {
        using (var connection = dbProviderFactory.CreateConnection())
        {
            connection!.ConnectionString = connectionString;
            await connection.OpenAsync();
            var executor = connection.CreateCommandAsyncExecutor();
            await executor.ExecuteReaderAsync(request, readResults);
        }
    }
}