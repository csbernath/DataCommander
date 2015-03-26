namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Text;
    using DataCommander.Foundation.Threading;

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
            Contract.Requires(command != null);

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
        /// <returns></returns>
        public static DataSet ExecuteDataSet(this IDbCommand command)
        {
            var dataSet = new DataSet();
            command.Fill(dataSet);
            return dataSet;
        }

        /// <summary>
        /// Execute the command and fills a <see cref="System.Data.DataTable"/>.
        /// </summary>T
        /// <param name="command"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(this IDbCommand command)
        {
            Contract.Requires(command != null);

            var dataTable = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };

            command.Fill(dataTable);
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
            Contract.Requires(command != null);

            object scalar = command.ExecuteScalar();
            Contract.Assert(scalar is T);

            return (T) scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command"></param>
        /// <returns></returns>
        public static T ExecuteScalarValueOrDefault<T>(this IDbCommand command)
        {
            Contract.Requires(command != null);

            object scalar = command.ExecuteScalar();
            return Database.GetValueOrDefault<T>(scalar);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public static int Fill(
            this IDbCommand command,
            DataSet dataSet)
        {
            Contract.Requires(command != null);
            Contract.Requires(dataSet != null);

            int rowCount = 0;
            int resultIndex = 0;
            DataTableCollection dataTables = dataSet.Tables;
            WorkerThread thread = WorkerThread.Current;

            if (!thread.IsStopRequested)
            {
                IDbConnection connection = command.Connection;

                using (ConnectionStateManager connectionStateManager = new ConnectionStateManager(connection))
                {
                    connectionStateManager.Open();

                    using (IDataReader reader = command.ExecuteReader())
                    {
                        while (true)
                        {
                            int fieldCount = reader.FieldCount;

                            if (fieldCount > 0)
                            {
                                DataTable table;

                                if (resultIndex < dataTables.Count)
                                {
                                    table = dataTables[resultIndex];
                                }
                                else
                                {
                                    table =
                                        new DataTable
                                        {
                                            Locale = CultureInfo.InvariantCulture
                                        };
                                    dataSet.Tables.Add(table);
                                }

                                int count = reader.Fill(table);
                                rowCount += count;
                            }

                            if (!thread.IsStopRequested)
                            {
                                bool nextResult = reader.NextResult();

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
        /// Fills the specified command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="dataTable">The data table.</param>
        /// <returns></returns>
        public static int Fill(
            this IDbCommand command,
            DataTable dataTable)
        {
            Contract.Requires(command != null);

            int rowCount = 0;
            WorkerThread thread = WorkerThread.Current;

            if (!thread.IsStopRequested)
            {
                IDbConnection connection = command.Connection;

                using (var connectionStateManager = new ConnectionStateManager(connection))
                {
                    connectionStateManager.Open();

                    try
                    {
                        using (IDataReader dataReader = command.ExecuteReader())
                        {
                            rowCount = dataReader.Fill(dataTable);
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
            Contract.Requires(command != null);

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