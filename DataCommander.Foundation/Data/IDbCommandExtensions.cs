namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// 
    /// </summary>
    public static class IDbCommandExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public static void AddParameterIfNotNull(this IDbCommand command, string parameterName, object value)
        {
#if CONTRACTS_FULL
            Contract.Requires(command != null);
#endif

            if (value != null)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = value;

                command.Parameters.Add(parameter);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(this IDbCommand command, CancellationToken cancellationToken)
        {
            var dataSet = new DataSet();
            command.Fill(dataSet, cancellationToken);
            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this IDbCommand command, CancellationToken cancellationToken)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
#endif

            var dataTable = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };

            command.Fill(dataTable, cancellationToken);
            return dataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static T ExecuteScalarValue<T>(this IDbCommand command)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
#endif

            var scalar = command.ExecuteScalar();
#if CONTRACTS_FULL
            Contract.Assert(scalar is T);
#endif

            return (T)scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static T ExecuteScalarValueOrDefault<T>(this IDbCommand command)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
#endif

            var scalar = command.ExecuteScalar();
            return Database.GetValueOrDefault<T>(scalar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataSet"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static int Fill(
            this IDbCommand command,
            DataSet dataSet,
            CancellationToken cancellationToken)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
            Contract.Requires<ArgumentNullException>(dataSet != null);
#endif

            var rowCount = 0;
            var resultIndex = 0;
            var dataTables = dataSet.Tables;

            if (!cancellationToken.IsCancellationRequested)
            {
                var connection = command.Connection;

                using (var connectionStateManager = new ConnectionStateManager(connection))
                {
                    connectionStateManager.Open();

                    using (var reader = command.ExecuteReader())
                    {
                        while (true)
                        {
                            var fieldCount = reader.FieldCount;

                            if (fieldCount > 0)
                            {
                                DataTable table;

                                if (resultIndex < dataTables.Count)
                                {
                                    table = dataTables[resultIndex];
                                }
                                else
                                {
                                    table = new DataTable
                                    {
                                        Locale = CultureInfo.InvariantCulture
                                    };
                                    dataSet.Tables.Add(table);
                                }

                                var count = reader.Fill(table, cancellationToken);
                                rowCount += count;
                            }

                            if (!cancellationToken.IsCancellationRequested)
                            {
                                var nextResult = reader.NextResult();

                                if (!nextResult)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }

                            resultIndex++;
                        }
                    }
                }
            }

            return rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataTable"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static int Fill(
            this IDbCommand command,
            DataTable dataTable,
            CancellationToken cancellationToken)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
#endif

            var rowCount = 0;

            if (!cancellationToken.IsCancellationRequested)
            {
                var connection = command.Connection;

                using (var connectionStateManager = new ConnectionStateManager(connection))
                {
                    connectionStateManager.Open();

                    try
                    {
                        using (var dataReader = command.ExecuteReader())
                        {
                            rowCount = dataReader.Fill(dataTable, cancellationToken);
                        }
                    }
                    catch (Exception exception)
                    {
                        throw new DbCommandExecutionException("IDbCommandExtensions.Fill failed.", exception, command);
                    }
                }
            }

            return rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public static string ToLogString(this IDbCommand command)
        {
#if CONTRACTS_FULL
            Contract.Requires<ArgumentNullException>(command != null);
#endif

            var sb = new StringBuilder();

            switch (command.CommandType)
            {
                case CommandType.StoredProcedure:
                    sb.Append("exec ");
                    break;

                default:
                    break;
            }

            sb.Append(command.CommandText);

            if (command.Parameters.Count > 0)
            {
                sb.AppendLine();
                sb.Append(command.Parameters.ToLogString());
            }

            return sb.ToString();
        }
    }
}