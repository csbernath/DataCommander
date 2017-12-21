using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class DbProviderFactoryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(
            this DbProviderFactory factory,
            DbConnection connection,
            string commandText)
        {
            FoundationContract.Requires<ArgumentNullException>(factory != null);
            FoundationContract.Requires<ArgumentNullException>(connection != null);

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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbProviderFactory"></param>
        /// <param name="connectionString"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="commandBehavior"></param>
        /// <param name="read"></param>
        /// <returns></returns>
        public static List<T> ExecuteReader<T>(
            this DbProviderFactory dbProviderFactory,
            string connectionString,
            CommandDefinition commandDefinition,
            CommandBehavior commandBehavior,
            Func<IDataRecord, T> read)
        {
            FoundationContract.Requires<ArgumentNullException>(dbProviderFactory != null);
            FoundationContract.Requires<ArgumentNullException>(commandDefinition != null);
            FoundationContract.Requires<ArgumentNullException>(read != null);

            using (var connection = dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);

                using (var dataReader = transactionScope.ExecuteReader(commandDefinition, commandBehavior))
                {
                    return dataReader.Read(read).ToList();
                }
            }
        }

        public static List<T> ExecuteReader<T>(
            this DbProviderFactory dbProviderFactory,
            string connectionString,
            ExecuteReaderRequest request,
            Func<IDataRecord, T> read)
        {
            using (var connection = dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var executor = new DbCommandExecutor(connection);
                return executor.ExecuteReader(request, read);
            }
        }
    }
}