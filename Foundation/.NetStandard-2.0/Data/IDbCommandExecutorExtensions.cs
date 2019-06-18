using System;
using System.Collections.Generic;
using System.Data;
using Foundation.Linq;

namespace Foundation.Data
{
    public static class DbCommandExecutorExtensions
    {
        private static void Execute(this IDbCommandExecutor executor, IEnumerable<ExecuteCommandRequest> requests)
        {
            executor.Execute(connection =>
            {
                foreach (var request in requests)
                    using (var command = connection.CreateCommand(request.CreateCommandRequest))
                        request.Execute(command);
            });
        }

        public static void Execute(this IDbCommandExecutor executor, CreateCommandRequest request, Action<IDbCommand> execute)
        {
            var requests = new ExecuteCommandRequest(request, execute).ItemToArray();
            executor.Execute(requests);
        }

        public static int ExecuteNonQuery(this IDbCommandExecutor executor, CreateCommandRequest request)
        {
            var affectedRows = 0;
            executor.Execute(request, command => affectedRows = command.ExecuteNonQuery());
            return affectedRows;
        }

        public static object ExecuteScalar(this IDbCommandExecutor executor, CreateCommandRequest request)
        {
            object scalar = null;
            executor.Execute(request, command => scalar = command.ExecuteScalar());
            return scalar;
        }

        public static void ExecuteReader(this IDbCommandExecutor executor, ExecuteReaderRequest request, Action<IDataReader> read)
        {
            executor.Execute(request.CreateCommandRequest, command =>
            {
                using (var dataReader = command.ExecuteReader(request.CommandBehavior))
                    read(dataReader);
            });
        }

        public static List<T> ExecuteReader<T>(this IDbCommandExecutor executor, ExecuteReaderRequest request, Func<IDataRecord, T> read)
        {
            List<T> rows = null;
            executor.ExecuteReader(request, dataReader => rows = dataReader.ReadResult(() => read(dataReader)));
            return rows;
        }

        public static DataTable ExecuteDataTable(this IDbCommandExecutor executor, ExecuteReaderRequest request)
        {
            DataTable dataTable = null;
            executor.Execute(request.CreateCommandRequest, command => { dataTable = command.ExecuteDataTable(request.CancellationToken); });
            return dataTable;
        }

        public static DataSet ExecuteDataSet(this IDbCommandExecutor executor, ExecuteReaderRequest request)
        {
            DataSet dataSet = null;
            executor.Execute(request.CreateCommandRequest, command => { dataSet = command.ExecuteDataSet(request.CancellationToken); });
            return dataSet;
        }
    }
}