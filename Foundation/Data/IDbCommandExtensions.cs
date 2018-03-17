using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using Foundation.Diagnostics;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data
{
    /// <summary>
    /// 
    /// </summary>
    public static class IDbCommandExtensions
    {
        public static void Initialize(this IDbCommand command, CreateCommandRequest request)
        {
            command.CommandType = request.CommandType;
            command.CommandText = request.CommandText;
            command.CommandTimeout = request.CommandTimeout;
            command.Transaction = request.Transaction;

            if (request.Parameters != null)
                command.Parameters.AddRange(request.Parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        public static void AddParameterIfNotNull(this IDbCommand command, string parameterName, object value)
        {
            Assert.IsNotNull(command);

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
            Assert.IsNotNull(command);

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
            Assert.IsNotNull(command);

            var scalar = command.ExecuteScalar();
            FoundationContract.Assert(scalar is T);
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
            Assert.IsNotNull(command);

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
            Assert.IsNotNull(command);
            Assert.IsNotNull(dataSet);

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
        public static int Fill(this IDbCommand command, DataTable dataTable, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(command);

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
            Assert.IsNotNull(command);

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