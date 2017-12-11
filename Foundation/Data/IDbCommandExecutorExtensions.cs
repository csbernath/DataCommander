using System;
using System.Collections.Generic;
using System.Data;

namespace Foundation.Data
{
    public static class IDbCommandExecutorExtensions
    {
        private static void Execute(this IDbCommandExecutor executor, IEnumerable<ExecuteCommandRequest> requests)
        {
            executor.Execute(connection =>
            {
                foreach (var request in requests)
                {
                    using (var command = connection.CreateCommand())
                    {
                        command.Initialize(request.CreateCommandRequest);
                        request.Execute(command);
                    }
                }
            });
        }

        public static void Execute(this IDbCommandExecutor executor, CreateCommandRequest request, Action<IDbCommand> execute)
        {
            var requests = new[]
            {
                new ExecuteCommandRequest(request, execute)
            };
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
            executor.ExecuteReader(request, dataReader => rows = dataReader.Read(() => read(dataReader)));
            return rows;
        }

        public static ExecuteReaderResponse<T1, T2> ExecuteReader<T1, T2>(this IDbCommandExecutor executor, ExecuteReaderRequest request,
            Func<IDataRecord, T1> read1, Func<IDataRecord, T2> read2)
        {
            ExecuteReaderResponse<T1, T2> response = null;
            executor.ExecuteReader(request, dataReader => response = dataReader.Read(() => read1(dataReader), () => read2(dataReader)));
            return response;
        }

        public static ExecuteReaderResponse<T1, T2, T3> ExecuteReader<T1, T2, T3>(this IDbCommandExecutor executor, ExecuteReaderRequest request,
            Func<IDataRecord, T1> read1, Func<IDataRecord, T2> read2, Func<IDataRecord, T3> read3)
        {
            ExecuteReaderResponse<T1, T2, T3> response = null;
            executor.ExecuteReader(request,
                dataReader => response = dataReader.Read(() => read1(dataReader), () => read2(dataReader), () => read3(dataReader)));
            return response;
        }

        public static ExecuteReaderResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> ExecuteReader<T1, T2, T3, T4, T5, T6, T7, T8, T9,
            T10, T11, T12, T13, T14, T15, T16, T17, T18, T19>(this IDbCommandExecutor executor, ExecuteReaderRequest request, Func<IDataRecord, T1> read1,
            Func<IDataRecord, T2> read2, Func<IDataRecord, T3> read3, Func<IDataRecord, T4> read4, Func<IDataRecord, T5> read5, Func<IDataRecord, T6> read6,
            Func<IDataRecord, T7> read7, Func<IDataRecord, T8> read8, Func<IDataRecord, T9> read9, Func<IDataRecord, T10> read10, Func<IDataRecord, T11> read11,
            Func<IDataRecord, T12> read12, Func<IDataRecord, T13> read13, Func<IDataRecord, T14> read14, Func<IDataRecord, T15> read15, Func<IDataRecord, T16> read16,
            Func<IDataRecord, T17> read17, Func<IDataRecord, T18> read18, Func<IDataRecord, T19> read19)
        {
            ExecuteReaderResponse<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, T17, T18, T19> response = null;
            executor.ExecuteReader(request,
                dataReader => response = dataReader.Read(() => read1(dataReader), () => read2(dataReader), () => read3(dataReader), () => read4(dataReader),
                    () => read5(dataReader), () => read6(dataReader), () => read7(dataReader), () => read8(dataReader), () => read9(dataReader), () => read10(dataReader),
                    () => read11(dataReader), () => read12(dataReader), () => read13(dataReader), () => read14(dataReader), () => read15(dataReader), () => read16(dataReader),
                    () => read17(dataReader), () => read18(dataReader), () => read19(dataReader)));
            return response;
        }
    }
}