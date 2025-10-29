using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using Foundation.Collections.ReadOnly;

namespace Foundation.Data;

public static class IDbCommandExecutorExtensions
{
    extension(IDbCommandExecutor executor)
    {
        private void Execute(IEnumerable<ExecuteCommandRequest> requests)
        {
            ArgumentNullException.ThrowIfNull(executor);

            executor.Execute(connection =>
            {
                foreach (var request in requests)
                    using (var command = connection.CreateCommand(request.CreateCommandRequest))
                        request.Execute(command);
            });
        }

        public void Execute(CreateCommandRequest request, Action<IDbCommand> execute)
        {
            ArgumentNullException.ThrowIfNull(executor);
            ArgumentNullException.ThrowIfNull(request);

            var executeCommandRequest = new ExecuteCommandRequest(request, execute);
            executor.Execute([executeCommandRequest]);
        }

        public int ExecuteNonQuery(CreateCommandRequest request)
        {
            ArgumentNullException.ThrowIfNull(executor);
            ArgumentNullException.ThrowIfNull(request);

            var affectedRows = 0;
            executor.Execute(request, command => affectedRows = command.ExecuteNonQuery());
            return affectedRows;
        }

        public object? ExecuteScalar(CreateCommandRequest request)
        {
            ArgumentNullException.ThrowIfNull(executor);
            ArgumentNullException.ThrowIfNull(request);

            object? scalar = null;
            executor.Execute(request, command => scalar = command.ExecuteScalar());
            return scalar;
        }

        public void ExecuteReader(ExecuteReaderRequest request, Action<IDataReader> readResults)
        {
            ArgumentNullException.ThrowIfNull(executor);
            ArgumentNullException.ThrowIfNull(request);

            executor.Execute(request.CreateCommandRequest, command =>
            {
                using var dataReader = command.ExecuteReader(request.CommandBehavior);
                readResults(dataReader);
            });
        }

        public ReadOnlySegmentLinkedList<T> ExecuteReader<T>(ExecuteReaderRequest request, int segmentLength,
            Func<IDataRecord, T> readRecord)
        {
            ArgumentNullException.ThrowIfNull(executor);

            ReadOnlySegmentLinkedList<T>? rows = null;
            executor.ExecuteReader(request, dataReader => rows = dataReader.ReadResult(segmentLength, readRecord));
            return rows!;
        }

        public DataTable ExecuteDataTable(ExecuteReaderRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(executor);

            DataTable? dataTable = null;
            executor.Execute(
                request.CreateCommandRequest,
                command => { dataTable = command.ExecuteDataTable(cancellationToken); });
            return dataTable!;
        }

        public DataSet ExecuteDataSet(ExecuteReaderRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(executor);
            ArgumentNullException.ThrowIfNull(request);

            DataSet? dataSet = null;
            executor.Execute(
                request.CreateCommandRequest,
                command => { dataSet = command.ExecuteDataSet(cancellationToken); });
            return dataSet!;
        }
    }
}