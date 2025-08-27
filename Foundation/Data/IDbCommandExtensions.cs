using System;
using System.Data;
using System.Globalization;
using System.Text;
using System.Threading;
using Foundation.Assertions;

namespace Foundation.Data;

public static class IDbCommandExtensions
{
    extension(IDbCommand command)
    {
        public void AddParameterIfNotNull(string parameterName, object value)
        {
            ArgumentNullException.ThrowIfNull(command);

            if (value != null)
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;
                parameter.Value = value;

                command.Parameters.Add(parameter);
            }
        }

        public DataSet ExecuteDataSet(CancellationToken cancellationToken)
        {
            var dataSet = new DataSet();
            command.Fill(dataSet, cancellationToken);
            return dataSet;
        }

        public DataTable ExecuteDataTable(CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);

            var dataTable = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };

            command.Fill(dataTable, cancellationToken);
            return dataTable;
        }

        public T? ExecuteScalarValue<T>()
        {
            ArgumentNullException.ThrowIfNull(command);

            var scalar = command.ExecuteScalar();
            Assert.IsTrue(scalar is T);
            return (T?)scalar;
        }

        public T? ExecuteScalarValueOrDefault<T>()
        {
            ArgumentNullException.ThrowIfNull(command);

            var scalar = command.ExecuteScalar();
            return ValueReader.GetValueOrDefault<T>(scalar);
        }

        public int Fill(DataSet dataSet, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);
            ArgumentNullException.ThrowIfNull(dataSet);

            var rowCount = 0;
            var resultIndex = 0;
            var dataTables = dataSet.Tables;

            if (!cancellationToken.IsCancellationRequested)
            {
                var connection = command.Connection!;

                using var connectionStateManager = new ConnectionStateManager(connection);
                connectionStateManager.Open();

                using var reader = command.ExecuteReader();
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

            return rowCount;
        }

        public int Fill(DataTable dataTable, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(command);

            var rowCount = 0;

            if (!cancellationToken.IsCancellationRequested)
            {
                var connection = command.Connection!;

                using var connectionStateManager = new ConnectionStateManager(connection);
                connectionStateManager.Open();

                try
                {
                    using var dataReader = command.ExecuteReader();
                    rowCount = dataReader.Fill(dataTable, cancellationToken);
                }
                catch (Exception exception)
                {
                    throw new DbCommandExecutionException("IDbCommandExtensions.Fill failed.", exception, command);
                }
            }

            return rowCount;
        }

        public string ToLogString()
        {
            ArgumentNullException.ThrowIfNull(command);

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

        internal void Initialize(CreateCommandRequest request)
        {
            command.CommandType = request.CommandType;
            command.CommandText = request.CommandText;

            if (request.CommandTimeout != null)
                command.CommandTimeout = request.CommandTimeout.Value;

            command.Transaction = request.Transaction;

            if (request.Parameters != null)
                command.Parameters.AddRange(request.Parameters);
        }
    }
}