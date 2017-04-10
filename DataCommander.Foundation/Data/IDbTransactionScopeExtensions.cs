namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public static class IDbTransactionScopeExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionScope"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand(this IDbTransactionScope transactionScope)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(transactionScope != null);
#endif
            var command = transactionScope.Connection.CreateCommand();
            command.Transaction = transactionScope.Transaction;
            return command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand(this IDbTransactionScope transactionScope, CommandDefinition commandDefinition)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(transactionScope != null);
            Contract.Requires<ArgumentNullException>(commandDefinition != null);
#endif

            var command = transactionScope.CreateCommand();
            command.CommandText = commandDefinition.CommandText;

            if (commandDefinition.Parameters != null)
            {
                command.Parameters.AddRange(commandDefinition.Parameters);
            }

            command.CommandType = commandDefinition.CommandType;
            command.CommandTimeout = commandDefinition.CommandTimeout;

            return command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="commandBehavior"></param>
        /// <param name="read"></param>
        /// <returns></returns>
        public static IEnumerable<T> ExecuteReader<T>(
            this IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition,
            CommandBehavior commandBehavior,
            Func<IDataRecord, T> read)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(transactionScope != null);
            Contract.Requires<ArgumentNullException>(read != null);
#endif

            using (var dataReader = transactionScope.ExecuteReader(commandDefinition, commandBehavior))
            {
                return dataReader.Read(read);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="commandBehavior"></param>
        /// <returns></returns>
        public static DataReader ExecuteReader(
            this IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition,
            CommandBehavior commandBehavior)
        {
            return DataReader.Create(transactionScope, commandDefinition, commandBehavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="commandBehavior"></param>
        /// <param name="read"></param>
        public static void ExecuteReader(
            this IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition,
            CommandBehavior commandBehavior,
            Action<IDataRecord> read)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(transactionScope != null);
            Contract.Requires<ArgumentNullException>(read != null);
#endif

            using (var dataReader = transactionScope.ExecuteReader(commandDefinition, commandBehavior))
            {
                dataReader.Read(read);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(
            this IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition,
            CancellationToken cancellationToken)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(transactionScope != null);
            Contract.Requires<ArgumentNullException>(commandDefinition != null);
#endif
            DataSet dataSet;

            using (var command = transactionScope.CreateCommand(commandDefinition))
            {
                dataSet = command.ExecuteDataSet(cancellationToken);
            }

            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(
            this IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition,
            CancellationToken cancellationToken)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(transactionScope != null);
            Contract.Requires<ArgumentNullException>(commandDefinition != null);
#endif
            DataTable dataTable;

            using (var command = transactionScope.CreateCommand(commandDefinition))
            {
                dataTable = command.ExecuteDataTable(cancellationToken);
            }

            return dataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery(
            this IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(transactionScope != null);
            Contract.Requires<ArgumentNullException>(commandDefinition != null);
#endif
            int affectedRowCount;

            using (var command = transactionScope.CreateCommand(commandDefinition))
            {
                affectedRowCount = command.ExecuteNonQuery();
            }

            return affectedRowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <returns></returns>
        public static object ExecuteScalar(
            this IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition)
        {
            object scalar;

            using (var command = transactionScope.CreateCommand(commandDefinition))
            {
                scalar = command.ExecuteScalar();
            }

            return scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="transactionScope"></param>
        /// <param name="commandDefinition"></param>
        /// <returns></returns>
        public static T ExecuteScalar<T>(
            this IDbTransactionScope transactionScope,
            CommandDefinition commandDefinition)
        {
            object scalar;

            using (var command = transactionScope.CreateCommand(commandDefinition))
            {
                scalar = command.ExecuteScalar();
            }

            return Database.GetValueOrDefault<T>(scalar);
        }
    }
}