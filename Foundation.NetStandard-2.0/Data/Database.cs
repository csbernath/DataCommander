using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Foundation.Diagnostics.Assertions;
using Foundation.Diagnostics.Contracts;

namespace Foundation.Data
{
    /// <summary>
    /// Helper base class for ADO.NET.
    /// </summary>
    public class Database : IDisposable
    {
        #region Private Fields

        private IDbProviderFactoryHelper _providerFactoryHelper;
        internal const string NullString = "null";

        #endregion

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        public Database()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="connection"></param>
        public Database(IDbConnection connection)
        {
            Connection = connection;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieves the underlying <see cref="System.Data.IDbConnection"/>.
        /// </summary>
        public IDbConnection Connection { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbProviderFactoryHelper ProviderFactoryHelper
        {
            get => _providerFactoryHelper;

            set
            {
                _providerFactoryHelper = value;
                CommandHelper = _providerFactoryHelper.DbCommandHelper;
                CommandBuilderHelper = _providerFactoryHelper.DbCommandBuilderHelper;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommandHelper CommandHelper { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommandBuilderHelper CommandBuilderHelper { get; private set; }

        /// <summary>
        /// Gets the underlying <see cref="IDbConnection.State"/>.
        /// </summary>
        public ConnectionState State => Connection.State;

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public IDbTransaction Transaction { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        /// <summary>
        /// Gets or sets the command timeout.
        /// </summary>
        /// <value>The command timeout.</value>
        public int CommandTimeout { [DebuggerStepThrough] get; [DebuggerStepThrough] set; }

        /// <summary>
        /// Gets the last executed command's rowcount.
        /// </summary>
        /// <remarks>
        /// <list type="table">
        /// <item>
        ///        <term>ExecuteNonQuery</term>
        ///        <description>number of affected rows. See <see cref="System.Data.IDbCommand.ExecuteNonQuery"/></description>
        /// </item>
        /// <item>
        ///        <term>Fill</term>
        ///        <description>number of rows read. See <see cref="System.Data.SqlClient.SqlDataAdapter"/>'s Fill method</description>
        /// </item>
        /// </list>
        /// </remarks>
        public int RowCount { get; private set; }

        /// <summary>
        /// Gets the last executed <see cref="System.Data.IDbCommand"/>.
        /// </summary>
        public IDbCommand Command { get; private set; }

        /// <summary>
        /// Gets the return value (int) of the last executed <see cref="System.Data.IDbCommand"/>.
        /// </summary>
        /// <remarks>
        /// The last executed command must be an SQL Server stored procedure.
        /// Gets the last executed command's 0th parameter's value as <see cref="System.Int32"/>
        /// </remarks>
        public int ReturnValue
        {
            get
            {
                Assert.IsNotNull(Command);
                FoundationContract.Requires<ArgumentException>(Command.Parameters.Count > 0);
                FoundationContract.Requires<ArgumentException>(Command.Parameters[0] is IDataParameter);
                FoundationContract.Requires<ArgumentException>(((IDataParameter)Command.Parameters[0]).Direction == ParameterDirection.ReturnValue);
                FoundationContract.Requires<ArgumentException>(((IDataParameter)Command.Parameters[0]).Value is int);

                var parameters = Command.Parameters;
                var parameterObject = parameters[0];
                var parameter = (IDataParameter) parameterObject;
                var valueObject = parameter.Value;
                var returnValue = (int) valueObject;
                return returnValue;
            }
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="connection"></param>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public static DataTable ExecuteDataTable(
            DbProviderFactory factory,
            DbConnection connection,
            string commandText)
        {
            Assert.IsNotNull(factory);
            Assert.IsNotNull(connection);

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            var adapter = factory.CreateDataAdapter();
            FoundationContract.Assert(adapter != null);

            adapter.SelectCommand = command;
            var table = new DataTable();
            adapter.Fill(table);
            return table;
        }

        /// <summary>
        /// writes into CSV file
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="textWriter"></param>
        public static void Write(
            DataTable dataTable,
            TextWriter textWriter)
        {
            Assert.IsNotNull(dataTable);
            Assert.IsNotNull(textWriter);

            var columns = dataTable.Columns;

            if (columns.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (DataColumn column in columns)
                {
                    sb.Append(column.ColumnName);
                    sb.Append('\t');
                }

                textWriter.WriteLine(sb);

                foreach (DataRow row in dataTable.Rows)
                {
                    sb.Length = 0;
                    var itemArray = row.ItemArray;
                    var last = itemArray.Length - 1;

                    for (var i = 0; i < last; i++)
                    {
                        sb.Append(itemArray[i]);
                        sb.Append('\t');
                    }

                    sb.Append(itemArray[last]);
                    textWriter.WriteLine(sb);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="columnSeparator"></param>
        /// <param name="lineSeparator"></param>
        /// <param name="textWriter"></param>
        public static void Write(
            DataView dataView,
            char columnSeparator,
            string lineSeparator,
            TextWriter textWriter)
        {
            Assert.IsValidOperation(!string.IsNullOrEmpty(lineSeparator));
            Assert.IsNotNull(textWriter);

            if (dataView != null)
            {
                var rowCount = dataView.Count;
                var dataTable = dataView.Table;
                var last = dataTable.Columns.Count - 1;

                for (var i = 0; i <= last; i++)
                {
                    var dataColumn = dataTable.Columns[i];
                    textWriter.Write(dataColumn.ColumnName);

                    if (i < last)
                    {
                        textWriter.Write(columnSeparator);
                    }
                    else
                    {
                        textWriter.Write(lineSeparator);
                    }
                }

                for (var i = 0; i < rowCount; i++)
                {
                    var dataRow = dataView[i].Row;
                    var itemArray = dataRow.ItemArray;

                    for (var j = 0; j <= last; j++)
                    {
                        textWriter.Write(itemArray[j]);

                        if (j < last)
                        {
                            textWriter.Write(columnSeparator);
                        }
                        else
                        {
                            textWriter.Write(lineSeparator);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="columnSeparator"></param>
        /// <param name="lineSeparator"></param>
        /// <returns></returns>
        public static string ToString(
            DataView dataView,
            char columnSeparator,
            string lineSeparator)
        {
            var textWriter = new StringWriter();
            Write(dataView, columnSeparator, lineSeparator, textWriter);
            var s = textWriter.ToString();
            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="dataRows"></param>
        /// <param name="textWriter"></param>
        public static void Write(
            DataTable dataTable,
            DataRow[] dataRows,
            TextWriter textWriter)
        {
            Assert.IsNotNull(dataTable);
            Assert.IsNotNull(dataRows);
            Assert.IsNotNull(textWriter);

            var columns = dataTable.Columns;

            if (columns.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (DataColumn column in columns)
                {
                    sb.Append(column.ColumnName);
                    sb.Append('\t');
                }

                textWriter.WriteLine(sb);

                foreach (var row in dataRows)
                {
                    sb.Length = 0;
                    var itemArray = row.ItemArray;
                    var last = itemArray.Length - 1;

                    for (var i = 0; i < last; i++)
                    {
                        sb.Append(itemArray[i]);
                        sb.Append('\t');
                    }

                    sb.Append(itemArray[last]);
                    textWriter.WriteLine(sb);
                }
            }
        }

        public static TResult GetValue<TResult>(object value, object inputNullValue, TResult outputNullValue)
        {
            TResult returnValue;

            if (value == null || value == inputNullValue)
                returnValue = outputNullValue;
            else
                returnValue = (TResult) value;

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="outputNullValue"></param>
        /// <returns></returns>
        public static TResult GetValue<TResult>(object value, TResult outputNullValue)
        {
            object inputNullValue = DBNull.Value;
            return GetValue(value, inputNullValue, outputNullValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TResult GetValueOrDefault<TResult>(object value)
        {
            object inputNullValue = DBNull.Value;
            var outputNullValue = default(TResult);
            return GetValue(value, inputNullValue, outputNullValue);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnNumber"></param>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        public static string ToString(
            DataTable dataTable,
            int columnNumber,
            int rowNumber)
        {
            Assert.IsNotNull(dataTable);

            var dataColumn = dataTable.Columns[columnNumber];
            var type = dataColumn.DataType;
            var typeCode = Type.GetTypeCode(type);
            var value = dataTable.Rows[rowNumber][columnNumber];
            string s;

            if (value == DBNull.Value)
            {
                s = "is null";
            }
            else
            {
                switch (typeCode)
                {
                    case TypeCode.String:
                        s = value.ToString().Replace("\'", "''");
                        s = string.Format(CultureInfo.InvariantCulture, "'{0}'", s);
                        break;

                    default:
                        s = value.ToString();
                        break;
                }
            }

            return s;
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Opens the underlying <see cref="System.Data.IDbConnection"/>
        /// </summary>
        public void Open()
        {
            Connection.Open();
        }

        /// <summary>
        /// Closes the underlying <see cref="System.Data.IDbConnection"/>.
        /// </summary>
        public void Close()
        {
            Connection.Close();
        }

        /// <summary>
        /// Disposes the underlying <see cref="System.Data.IDbConnection"/>
        /// </summary>
        public void Dispose()
        {
            Connection.Dispose();
        }

        /// <summary>
        ///  Begins a database transaction.
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            Assert.IsNotNull(Connection);

            return Connection.BeginTransaction();
        }

        /// <summary>
        /// Begins a database transaction with the specified System.Data.IsolationLevel value.
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            Assert.IsNotNull(Connection);

            return Connection.BeginTransaction(il);
        }

        /// <summary>
        /// Creates and returns a <see cref="System.Data.IDbCommand"/> objects associated with the connection. 
        /// </summary>
        /// <remarks>
        /// Sets the CommandTimeout property.
        /// </remarks>
        /// <returns>
        /// A <see cref="System.Data.IDbCommand"/> object associated with the connection.
        /// </returns>
        public IDbCommand CreateCommand()
        {
            FoundationContract.Requires<ArgumentException>(Connection != null);

            Command = Connection.CreateCommand();
            Command.Transaction = Transaction;
            Command.CommandTimeout = CommandTimeout;
            return Command;
        }

        /// <summary>
        /// Creates and returns a Command object associated with the connection.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>A <see cref="System.Data.IDbCommand"/> object associated with the connection.</returns>
        public IDbCommand CreateCommand(string commandText)
        {
            Command = CreateCommand();
            Command.CommandText = commandText;
            return Command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns>A Command object associated with the connection.</returns>
        public IDbCommand CreateCommand(string commandText, CommandType commandType)
        {
            Command = CreateCommand(commandText);
            Command.CommandType = commandType;
            return Command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void DeriveParameters(IDbCommand command)
        {
            Assert.IsNotNull(CommandBuilderHelper);

            CommandBuilderHelper.DeriveParameters(command);
        }

        /// <summary>
        /// Executes an SQL statement against the Connection object of a .NET Framework data provider, and returns the number of rows affected.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(IDbCommand command)
        {
            Assert.IsNotNull(command);

            Command = command;
            RowCount = command.ExecuteNonQuery();
            return RowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int ExecuteNonQuery(string commandText)
        {
            //this.command = this.CreateCommand(commandText);
            //this.rowCount = this.command.ExecuteNonQuery();
            //return this.rowCount;

            Command = CreateCommand(commandText);
            try
            {
                RowCount = Command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new DbCommandExecutionException("Database.ExecuteNonQuery failed.", exception, Command);
            }

            return RowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public object ExecuteScalar(IDbCommand command)
        {
            Assert.IsNotNull(command);

            Command = command;
            var scalar = command.ExecuteScalar();
            return scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText)
        {
            Command = CreateCommand(commandText);
            var scalar = Command.ExecuteScalar();
            return scalar;
        }

        /// <summary>
        /// Executes the commandText against the <see cref="Connection"/> and builds an <see cref="System.Data.IDataReader"/>.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string commandText)
        {
            Command = CreateCommand(commandText);
            return Command.ExecuteReader();
        }

        /// <summary>
        /// Executes the commandText against the Connection, and builds an <see cref="IDataReader"/> using one of the <see cref="CommandBehavior"/> values.
        /// </summary>
        /// <param name="commandText">The text command to execute.</param>
        /// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
        /// <returns>An <see cref="IDataReader"/> object.</returns>
        public IDataReader ExecuteReader(string commandText, CommandBehavior behavior)
        {
            Command = CreateCommand(commandText);
            return Command.ExecuteReader(behavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, CancellationToken cancellationToken)
        {
            Command = CreateCommand(commandText);
            return Command.ExecuteDataTable(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(IDbCommand command, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(command);

            Command = command;
            var dataTable = new DataTable();
            RowCount = command.Fill(dataTable, cancellationToken);
            return dataTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(IDbCommand command, CancellationToken cancellationToken)
        {
            Command = command;
            var dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            RowCount = command.Fill(dataSet, cancellationToken);
            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet(string commandText, CancellationToken cancellationToken)
        {
            Command = CreateCommand(commandText);
            return ExecuteDataSet(Command, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public XmlDocument ExecuteXmlDocument(IDbCommand command)
        {
            Assert.IsNotNull(command);
            Assert.IsNotNull(CommandHelper);

            return CommandHelper.ExecuteXmlDocument(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public XmlDocument ExecuteXmlDocument(string commandText)
        {
            Command = CreateCommand(commandText);
            return ExecuteXmlDocument(Command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public DataTable FillSchema(IDbCommand command, DataTable dataTable)
        {
            Assert.IsNotNull(command);

            Command = command;
            DataTable schemaTable;

            using (var dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
            {
                schemaTable = FillSchema(dataReader, dataTable);
            }

            return schemaTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataSet"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static DataTable[] FillSchema(IDbCommand command, DataSet dataSet, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(command);

            DataTable[] schemaTables;

            using (var dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
            {
                schemaTables = FillSchema(dataReader, dataSet, cancellationToken);
            }

            return schemaTables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="dataSet"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public int Fill(string commandText, DataSet dataSet, CancellationToken cancellationToken)
        {
            Assert.IsNotNull(dataSet);

            Command = CreateCommand(commandText);
            RowCount = Command.Fill(dataSet, cancellationToken);
            return RowCount;
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Inherited class must call this method first.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <param name="commandHelper"></param>
        /// <param name="commandTimeout"></param>
        protected void Initialize(
            IDbConnection connection,
            IDbTransaction transaction,
            IDbCommandHelper commandHelper,
            int commandTimeout)
        {
            Connection = connection;
            Transaction = transaction;
            CommandHelper = commandHelper;
            CommandTimeout = commandTimeout;
        }

        #endregion

        #region Private Methods

        internal static void FillSchema(DataTable schemaTable, DataTable dataTable)
        {
            var primaryKey = new List<DataColumn>();
            var columns = dataTable.Columns;
            var isKeyColumn = columns["IsKey"];

            foreach (DataRow row in schemaTable.Rows)
            {
                var columnName = (string) row["ColumnName"];
                var dataType = (Type) row["DataType"];
                var isKey = isKeyColumn != null && row.GetNullableValueField<bool>(isKeyColumn) == true;
                var columnNameAdd = columnName;
                var index = 2;

                while (true)
                {
                    if (columns.Contains(columnNameAdd))
                    {
                        columnNameAdd = $"{columnName}{index}";
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                var column = new DataColumn(columnNameAdd, dataType);
                columns.Add(column);

                if (isKey)
                {
                    primaryKey.Add(column);
                }
            }

            var array = primaryKey.ToArray();
            dataTable.PrimaryKey = array;
        }

        private static DataTable FillSchema(
            IDataReader dataReader,
            DataTable dataTable)
        {
            var schemaTable = dataReader.GetSchemaTable();
            FillSchema(schemaTable, dataTable);
            return schemaTable;
        }

        private static DataTable[] FillSchema(
            IDataReader dataReader,
            DataSet dataSet,
            CancellationToken cancellationToken)
        {
            var schemaTables = new List<DataTable>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var dataTable = new DataTable {Locale = CultureInfo.InvariantCulture};
                var schemaTable = FillSchema(dataReader, dataTable);
                dataSet.Tables.Add(dataTable);
                schemaTables.Add(schemaTable);

                if (!dataReader.NextResult())
                {
                    break;
                }
            }

            var array = schemaTables.ToArray();
            return array;
        }

        #endregion
    }
}