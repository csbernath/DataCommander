using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using Foundation.Assertions;

namespace Foundation.Data
{
    public static class DbProviderFactoryExtensions
    {
        public static DataTable ExecuteDataTable(this DbProviderFactory factory, DbConnection connection, string commandText)
        {
            Assert.IsNotNull(factory);
            Assert.IsNotNull(connection);

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            var adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            var table = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };
            adapter.Fill(table);
            return table;
        }

        public static void ExecuteReader(this DbProviderFactory dbProviderFactory, string connectionString, ExecuteReaderRequest request,
            Action<IDataReader> read)
        {
            using (var connection = dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var executor = connection.CreateCommandExecutor();
                executor.ExecuteReader(request, read);
            }
        }

        public static List<T> ExecuteReader<T>(this DbProviderFactory dbProviderFactory, string connectionString, ExecuteReaderRequest request,
            Func<IDataRecord, T> read)
        {
            List<T> rows = null;
            dbProviderFactory.ExecuteReader(connectionString, request, dataReader => rows = dataReader.ReadResult(read));
            return rows;
        }

        public static void ExecuteTransaction(this DbProviderFactory dbProviderFactory, string connectionString, Action<IDbTransaction> action)
        {
            using (var connection = dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    action(transaction);
                    transaction.Commit();
                }
            }
        }
    }
}