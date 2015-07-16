namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Linq;

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
            Contract.Requires<ArgumentNullException>(factory != null);
            Contract.Requires<ArgumentNullException>(connection != null);

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
        /// <param name="dbProviderFactory"></param>
        /// <param name="connectionString"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="commandBehavior"></param>
        /// <param name="read"></param>
        public static IEnumerable<T> ExecuteReader<T>(
            this DbProviderFactory dbProviderFactory,
            string connectionString,
            CommandDefinition commandDefinition,
            CommandBehavior commandBehavior,
            Func<IDataRecord, T> read)
        {
            Contract.Requires<ArgumentNullException>(dbProviderFactory != null);
            Contract.Requires<ArgumentNullException>(commandDefinition != null);
            Contract.Requires<ArgumentNullException>(read != null);

            using (var connection = dbProviderFactory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                var transactionScope = new DbTransactionScope(connection, null);

                using (var dataReader = transactionScope.ExecuteReader(commandDefinition, commandBehavior))
                {
                    return dataReader.Read(read);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbProviderFactory"></param>
        /// <param name="connnectionString"></param>
        /// <param name="commandText"></param>
        /// <param name="read"></param>
        public static IEnumerable<T> ExecuteReader<T>(
            this DbProviderFactory dbProviderFactory,
            string connnectionString,
            string commandText,
            Func<IDataRecord, T> read)
        {
            return dbProviderFactory.ExecuteReader(connnectionString, new CommandDefinition {CommandText = commandText}, CommandBehavior.Default, read);
        }
    }
}