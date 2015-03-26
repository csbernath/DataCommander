namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;
#if DAPPER
    using System.Collections.Generic;
    using Dapper;
#endif

    /// <summary>
    /// 
    /// </summary>
    public static class IDbConnectionContextExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static IDbCommand CreateCommand( this IDbConnectionContext connectionContext, string commandText, CommandType commandType )
        {
            Contract.Requires<ArgumentNullException>( connectionContext != null );
            Contract.Requires<ArgumentNullException>( connectionContext.Connection != null );
            var connection = connectionContext.Connection;

            return connection.CreateCommand( connectionContext.Transaction, commandText, commandType, connectionContext.CommandTimeout );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet( this IDbConnectionContext connectionContext, string commandText, CommandType commandType )
        {
            Contract.Requires<ArgumentNullException>( connectionContext != null );
            Contract.Requires<ArgumentNullException>( connectionContext.Connection != null );
            var connection = connectionContext.Connection;

            return connection.ExecuteDataSet( connectionContext.Transaction, commandText, commandType, connectionContext.CommandTimeout );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable( this IDbConnectionContext connectionContext, string commandText, CommandType commandType )
        {
            Contract.Requires<ArgumentNullException>( connectionContext != null );
            Contract.Requires<ArgumentNullException>( connectionContext.Connection != null );
            var connection = connectionContext.Connection;

            return connection.ExecuteDataTable( connectionContext.Transaction, commandText, commandType, connectionContext.CommandTimeout );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static int ExecuteNonQuery( this IDbConnectionContext connectionContext, string commandText, CommandType commandType )
        {
            Contract.Requires<ArgumentNullException>( connectionContext != null );
            Contract.Requires<ArgumentNullException>( connectionContext.Connection != null );
            var connection = connectionContext.Connection;

            return connection.ExecuteNonQuery( connectionContext.Transaction, commandText, commandType, connectionContext.CommandTimeout );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static object ExecuteScalar( this IDbConnectionContext connectionContext, string commandText, CommandType commandType )
        {
            Contract.Requires<ArgumentNullException>( connectionContext != null );
            Contract.Requires<ArgumentNullException>( connectionContext.Connection != null );
            var connection = connectionContext.Connection;

            return connection.ExecuteScalar( connectionContext.Transaction, commandText, commandType, connectionContext.CommandTimeout );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <param name="behavior"></param>
        /// <returns></returns>
        public static IDataReaderContext ExecuteReader( this IDbConnectionContext connectionContext, string commandText, CommandType commandType, CommandBehavior behavior )
        {
            Contract.Requires<ArgumentNullException>( connectionContext != null );
            Contract.Requires<ArgumentNullException>( connectionContext.Connection != null );

            var command = connectionContext.CreateCommand( commandText, commandType );
            IDataReader dataReader;

            try
            {
                dataReader = command.ExecuteReader( behavior );
            }
            catch
            {
                if (command != null)
                {
                    command.Dispose();
                }

                throw;
            }

            return new DataReaderContext( command, dataReader );
        }

#if DAPPER
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connectionContext"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <param name="buffered"></param>
        /// <returns></returns>
        public static IEnumerable<T> Query<T>(
            this IDbConnectionContext connectionContext,
            string sql,
            object param,
            bool buffered,
            CommandType? commandType )
        {
            Contract.Requires<ArgumentNullException>( connectionContext != null );
            Contract.Requires<ArgumentNullException>( connectionContext.Connection != null );

            var connection = connectionContext.Connection;

            return connection.Query<T>( sql, param, connectionContext.Transaction, buffered, connectionContext.CommandTimeout, commandType );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connectionContext"></param>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public static SqlMapper.GridReader QueryMultiple(
            this IDbConnectionContext connectionContext,
            string sql,
            object param,
            CommandType? commandType )
        {
            Contract.Requires<ArgumentNullException>( connectionContext != null );
            Contract.Requires<ArgumentNullException>( connectionContext.Connection != null );

            var connection = connectionContext.Connection;

            return connection.QueryMultiple( sql, param, connectionContext.Transaction, connectionContext.CommandTimeout, commandType );
        }
#endif
    }
}