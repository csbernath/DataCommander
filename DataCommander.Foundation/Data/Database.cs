namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Threading;
    using System.Xml;

    /// <summary>
    /// Helper base class for ADO.NET.
    /// </summary>
    public class Database : IDisposable
    {
        #region Private Fields

        private IDbProviderFactoryHelper providerFactoryHelper;
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
            this.Connection = connection;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieves the underlying <see cref="System.Data.IDbConnection"/>.
        /// </summary>
        public IDbConnection Connection
        {
            [DebuggerStepThrough]
            get;

            [DebuggerStepThrough]
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbProviderFactoryHelper ProviderFactoryHelper
        {
            get
            {
                return this.providerFactoryHelper;
            }

            set
            {
                this.providerFactoryHelper = value;
                this.CommandHelper = this.providerFactoryHelper.DbCommandHelper;
                this.CommandBuilderHelper = this.providerFactoryHelper.DbCommandBuilderHelper;
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
        public ConnectionState State => this.Connection.State;

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public IDbTransaction Transaction
        {
            [DebuggerStepThrough]
            get;

            [DebuggerStepThrough]
            set;
        }

        /// <summary>
        /// Gets or sets the command timeout.
        /// </summary>
        /// <value>The command timeout.</value>
        public int CommandTimeout
        {
            [DebuggerStepThrough]
            get;

            [DebuggerStepThrough]
            set;
        }

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
                Contract.Requires<ArgumentNullException>(this.Command != null);
                Contract.Requires<ArgumentException>(this.Command.Parameters.Count > 0);
                Contract.Requires<ArgumentException>(this.Command.Parameters[0] is IDataParameter);
                Contract.Requires<ArgumentException>(((IDataParameter)this.Command.Parameters[0]).Direction == ParameterDirection.ReturnValue);
                Contract.Requires<ArgumentException>(((IDataParameter)this.Command.Parameters[0]).Value is int);

                IDataParameterCollection parameters = this.Command.Parameters;
                object parameterObject = parameters[0];
                var parameter = (IDataParameter)parameterObject;
                object valueObject = parameter.Value;
                int returnValue = (int)valueObject;
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
            Contract.Requires<ArgumentNullException>(factory != null);
            Contract.Requires<ArgumentNullException>(connection != null);

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            var adapter = factory.CreateDataAdapter();
            Contract.Assert(adapter != null);
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
            Contract.Requires<ArgumentNullException>(dataTable != null);
            Contract.Requires<ArgumentNullException>(textWriter != null);

            DataColumnCollection columns = dataTable.Columns;

            if (columns.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (DataColumn column in columns)
                {
                    sb.Append(column.ColumnName);
                    sb.Append('\t');
                }

                textWriter.WriteLine(sb);

                foreach (DataRow row in dataTable.Rows)
                {
                    sb.Length = 0;
                    object[] itemArray = row.ItemArray;
                    int last = itemArray.Length - 1;

                    for (int i = 0; i < last; i++)
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
            Contract.Requires(!string.IsNullOrEmpty(lineSeparator));
            Contract.Requires(textWriter != null);

            if (dataView != null)
            {
                int rowCount = dataView.Count;
                DataTable dataTable = dataView.Table;
                int last = dataTable.Columns.Count - 1;

                for (int i = 0; i <= last; i++)
                {
                    DataColumn dataColumn = dataTable.Columns[i];
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

                for (int i = 0; i < rowCount; i++)
                {
                    DataRow dataRow = dataView[i].Row;
                    object[] itemArray = dataRow.ItemArray;

                    for (int j = 0; j <= last; j++)
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
            Char columnSeparator,
            string lineSeparator)
        {
            StringWriter textWriter = new StringWriter();
            Write(dataView, columnSeparator, lineSeparator, textWriter);
            string s = textWriter.ToString();
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
            Contract.Requires<ArgumentNullException>(dataTable != null);
            Contract.Requires<ArgumentNullException>(dataRows != null);
            Contract.Requires<ArgumentNullException>(textWriter != null);

            DataColumnCollection columns = dataTable.Columns;

            if (columns.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (DataColumn column in columns)
                {
                    sb.Append(column.ColumnName);
                    sb.Append('\t');
                }

                textWriter.WriteLine(sb);

                foreach (DataRow row in dataRows)
                {
                    sb.Length = 0;
                    object[] itemArray = row.ItemArray;
                    int last = itemArray.Length - 1;

                    for (int i = 0; i < last; i++)
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
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <param name="inputNullValue"></param>
        /// <param name="outputNullValue"></param>
        /// <returns></returns>
        public static TResult GetValue<TResult>(object value, object inputNullValue, TResult outputNullValue)
        {
            TResult returnValue;

            if (value == null || value == inputNullValue)
            {
                returnValue = outputNullValue;
            }
            else
            {
                returnValue = (TResult)value;
            }

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
            TResult outputNullValue = default(TResult);
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
            Contract.Requires<ArgumentNullException>(dataTable != null);

            DataColumn dataColumn = dataTable.Columns[columnNumber];
            Type type = dataColumn.DataType;
            TypeCode typeCode = Type.GetTypeCode(type);
            object value = dataTable.Rows[rowNumber][columnNumber];
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
            this.Connection.Open();
        }

        /// <summary>
        /// Closes the underlying <see cref="System.Data.IDbConnection"/>.
        /// </summary>
        public void Close()
        {
            this.Connection.Close();
        }

        /// <summary>
        /// Disposes the underlying <see cref="System.Data.IDbConnection"/>
        /// </summary>
        public void Dispose()
        {
            this.Connection.Dispose();
        }

        /// <summary>
        ///  Begins a database transaction.
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            Contract.Requires(this.Connection != null);

            return this.Connection.BeginTransaction();
        }

        /// <summary>
        /// Begins a database transaction with the specified System.Data.IsolationLevel value.
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            Contract.Requires(this.Connection != null);

            return this.Connection.BeginTransaction(il);
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
            Contract.Requires(this.Connection != null);

            this.Command = this.Connection.CreateCommand();
            this.Command.Transaction = this.Transaction;
            this.Command.CommandTimeout = this.CommandTimeout;
            return this.Command;
        }

        /// <summary>
        /// Creates and returns a Command object associated with the connection.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>A <see cref="System.Data.IDbCommand"/> object associated with the connection.</returns>
        public IDbCommand CreateCommand(string commandText)
        {
            this.Command = this.CreateCommand();
            this.Command.CommandText = commandText;
            return this.Command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns>A Command object associated with the connection.</returns>
        public IDbCommand CreateCommand(string commandText, CommandType commandType)
        {
            this.Command = this.CreateCommand(commandText);
            this.Command.CommandType = commandType;
            return this.Command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void DeriveParameters(IDbCommand command)
        {
            Contract.Requires(this.CommandBuilderHelper != null);

            this.CommandBuilderHelper.DeriveParameters(command);
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
            Contract.Requires(command != null);

            this.Command = command;
            this.RowCount = command.ExecuteNonQuery();
            return this.RowCount;
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

            this.Command = this.CreateCommand(commandText);
            try
            {
                this.RowCount = this.Command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new DbCommandExecutionException("Database.ExecuteNonQuery failed.", exception, this.Command);
            }

            return this.RowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public object ExecuteScalar(IDbCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            this.Command = command;
            object scalar = command.ExecuteScalar();
            return scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public object ExecuteScalar(string commandText)
        {
            this.Command = this.CreateCommand(commandText);
            object scalar = this.Command.ExecuteScalar();
            return scalar;
        }

        /// <summary>
        /// Executes the commandText against the <see cref="Connection"/> and builds an <see cref="System.Data.IDataReader"/>.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader(string commandText)
        {
            this.Command = this.CreateCommand(commandText);
            return this.Command.ExecuteReader();
        }

        /// <summary>
        /// Executes the commandText against the Connection, and builds an <see cref="IDataReader"/> using one of the <see cref="CommandBehavior"/> values.
        /// </summary>
        /// <param name="commandText">The text command to execute.</param>
        /// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
        /// <returns>An <see cref="IDataReader"/> object.</returns>
        public IDataReader ExecuteReader(string commandText, CommandBehavior behavior)
        {
            this.Command = this.CreateCommand(commandText);
            return this.Command.ExecuteReader(behavior);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(string commandText, CancellationToken cancellationToken)
        {
            this.Command = this.CreateCommand(commandText);
            return this.Command.ExecuteDataTable(cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable(IDbCommand command, CancellationToken cancellationToken)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            this.Command = command;
            var dataTable = new DataTable();
            this.RowCount = command.Fill(dataTable, cancellationToken);
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
            this.Command = command;
            var dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            this.RowCount = command.Fill(dataSet, cancellationToken);
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
            this.Command = this.CreateCommand(commandText);
            return this.ExecuteDataSet(this.Command, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public XmlDocument ExecuteXmlDocument(IDbCommand command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            Contract.Requires<ArgumentNullException>(this.CommandHelper != null);

            return this.CommandHelper.ExecuteXmlDocument(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public XmlDocument ExecuteXmlDocument(string commandText)
        {
            this.Command = this.CreateCommand(commandText);
            return this.ExecuteXmlDocument(this.Command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public DataTable FillSchema(IDbCommand command, DataTable dataTable)
        {
            Contract.Requires<ArgumentNullException>(command != null);

            this.Command = command;
            DataTable schemaTable;

            using (IDataReader dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
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
            Contract.Requires<ArgumentNullException>(command != null);

            DataTable[] schemaTables;

            using (IDataReader dataReader = command.ExecuteReader(CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo))
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
            Contract.Requires<ArgumentNullException>(dataSet != null);

            this.Command = this.CreateCommand(commandText);
            this.RowCount = this.Command.Fill(dataSet, cancellationToken);
            return this.RowCount;
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
            this.Connection = connection;
            this.Transaction = transaction;
            this.CommandHelper = commandHelper;
            this.CommandTimeout = commandTimeout;
        }

        #endregion

        #region Private Methods

        internal static void FillSchema(DataTable schemaTable, DataTable dataTable)
        {
            List<DataColumn> primaryKey = new List<DataColumn>();
            DataColumnCollection columns = dataTable.Columns;
            DataColumn isKeyColumn = columns["IsKey"];

            foreach (DataRow row in schemaTable.Rows)
            {
                string columnName = (string)row["ColumnName"];
                Type dataType = (Type)row["DataType"];
                bool isKey = isKeyColumn != null && row.Field<bool?>(isKeyColumn) == true;
                string columnNameAdd = columnName;
                int index = 2;

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

                DataColumn column = new DataColumn(columnNameAdd, dataType);
                columns.Add(column);

                if (isKey)
                {
                    primaryKey.Add(column);
                }
            }

            DataColumn[] array = primaryKey.ToArray();
            dataTable.PrimaryKey = array;
        }

        private static DataTable FillSchema(
            IDataReader dataReader,
            DataTable dataTable)
        {
            DataTable schemaTable = dataReader.GetSchemaTable();
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
                DataTable schemaTable = FillSchema(dataReader, dataTable);
                dataSet.Tables.Add(dataTable);
                schemaTables.Add(schemaTable);

                if (!dataReader.NextResult())
                {
                    break;
                }
            }

            DataTable[] array = schemaTables.ToArray();
            return array;
        }

        #endregion
    }
}