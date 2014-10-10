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
    using System.Xml;
    using DataCommander.Foundation.Data.SqlClient;
    using DataCommander.Foundation.Text;
    using DataCommander.Foundation.Threading;

    /// <summary>
    /// Helper base class for ADO.NET.
    /// </summary>
    public class Database : IDisposable
    {
        #region Private Fields

        private IDbConnection connection;
        private IDbTransaction transaction;
        private IDbProviderFactoryHelper providerFactoryHelper;
        private IDbCommandHelper commandHelper;
        private IDbCommandBuilderHelper commandBuilderHelper;
        private Int32 commandTimeout;
        private Int32 rowCount;
        private IDbCommand command;
        internal const String NullString = "null";

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
        public Database( IDbConnection connection )
        {
            this.connection = connection;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Retrieves the underlying <see cref="System.Data.IDbConnection"/>.
        /// </summary>
        public IDbConnection Connection
        {
            [DebuggerStepThrough]
            get
            {
                return this.connection;
            }

            [DebuggerStepThrough]
            set
            {
                this.connection = value;
            }
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
                this.commandHelper = this.providerFactoryHelper.DbCommandHelper;
                this.commandBuilderHelper = this.providerFactoryHelper.DbCommandBuilderHelper;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommandHelper CommandHelper
        {
            get
            {
                return this.commandHelper;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDbCommandBuilderHelper CommandBuilderHelper
        {
            get
            {
                return this.commandBuilderHelper;
            }
        }

        /// <summary>
        /// Gets the underlying <see cref="IDbConnection.State"/>.
        /// </summary>
        public ConnectionState State
        {
            get
            {
                return this.connection.State;
            }
        }

        /// <summary>
        /// Gets or sets the transaction.
        /// </summary>
        /// <value>The transaction.</value>
        public IDbTransaction Transaction
        {
            [DebuggerStepThrough]
            get
            {
                return this.transaction;
            }

            [DebuggerStepThrough]
            set
            {
                this.transaction = value;
            }
        }

        /// <summary>
        /// Gets or sets the command timeout.
        /// </summary>
        /// <value>The command timeout.</value>
        public Int32 CommandTimeout
        {
            [DebuggerStepThrough]
            get
            {
                return this.commandTimeout;
            }

            [DebuggerStepThrough]
            set
            {
                this.commandTimeout = value;
            }
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
        public Int32 RowCount
        {
            get
            {
                return this.rowCount;
            }
        }

        /// <summary>
        /// Gets the last executed <see cref="System.Data.IDbCommand"/>.
        /// </summary>
        public IDbCommand Command
        {
            get
            {
                return this.command;
            }
        }

        /// <summary>
        /// Gets the return value (Int32) of the last executed <see cref="System.Data.IDbCommand"/>.
        /// </summary>
        /// <remarks>
        /// The last executed command must be an SQL Server stored procedure.
        /// Gets the last executed command's 0th parameter's value as <see cref="System.Int32"/>
        /// </remarks>
        public Int32 ReturnValue
        {
            get
            {
                Contract.Requires( this.Command != null );
                Contract.Requires( this.Command.Parameters.Count > 0 );
                Contract.Requires( this.Command.Parameters[ 0 ] is IDataParameter );
                Contract.Requires( ( (IDataParameter)this.Command.Parameters[ 0 ] ).Direction == ParameterDirection.ReturnValue );
                Contract.Requires( ( (IDataParameter)this.Command.Parameters[ 0 ] ).Value is Int32 );

                IDataParameterCollection parameters = this.command.Parameters;
                Object parameterObject = parameters[ 0 ];
                IDataParameter parameter = (IDataParameter)parameterObject;
                Object valueObject = parameter.Value;
                Int32 returnValue = (Int32)valueObject;
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
            String commandText )
        {
            Contract.Requires( factory != null );
            Contract.Requires( connection != null );

            var command = connection.CreateCommand();
            command.CommandText = commandText;
            var adapter = factory.CreateDataAdapter();
            adapter.SelectCommand = command;
            var table = new DataTable();
            adapter.Fill( table );
            return table;
        }

        /// <summary>
        /// writes into CSV file
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="textWriter"></param>
        public static void Write(
            DataTable dataTable,
            TextWriter textWriter )
        {
            Contract.Requires( dataTable != null );
            Contract.Requires( textWriter != null );

            DataColumnCollection columns = dataTable.Columns;

            if (columns.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (DataColumn column in columns)
                {
                    sb.Append( column.ColumnName );
                    sb.Append( '\t' );
                }

                textWriter.WriteLine( sb );

                foreach (DataRow row in dataTable.Rows)
                {
                    sb.Length = 0;
                    Object[] itemArray = row.ItemArray;
                    Int32 last = itemArray.Length - 1;

                    for (Int32 i = 0; i < last; i++)
                    {
                        sb.Append( itemArray[ i ] );
                        sb.Append( '\t' );
                    }

                    sb.Append( itemArray[ last ] );
                    textWriter.WriteLine( sb );
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
            Char columnSeparator,
            String lineSeparator,
            TextWriter textWriter )
        {
            Contract.Requires( !string.IsNullOrEmpty( lineSeparator ) );
            Contract.Requires( textWriter != null );

            if (dataView != null)
            {
                Int32 rowCount = dataView.Count;
                DataTable dataTable = dataView.Table;
                Int32 last = dataTable.Columns.Count - 1;

                for (Int32 i = 0; i <= last; i++)
                {
                    DataColumn dataColumn = dataTable.Columns[ i ];
                    textWriter.Write( dataColumn.ColumnName );

                    if (i < last)
                    {
                        textWriter.Write( columnSeparator );
                    }
                    else
                    {
                        textWriter.Write( lineSeparator );
                    }
                }

                for (Int32 i = 0; i < rowCount; i++)
                {
                    DataRow dataRow = dataView[ i ].Row;
                    Object[] itemArray = dataRow.ItemArray;

                    for (Int32 j = 0; j <= last; j++)
                    {
                        textWriter.Write( itemArray[ j ] );

                        if (j < last)
                        {
                            textWriter.Write( columnSeparator );
                        }
                        else
                        {
                            textWriter.Write( lineSeparator );
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
        public static String ToString(
            DataView dataView,
            Char columnSeparator,
            String lineSeparator )
        {
            StringWriter textWriter = new StringWriter();
            Write( dataView, columnSeparator, lineSeparator, textWriter );
            String s = textWriter.ToString();
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
            TextWriter textWriter )
        {
            Contract.Requires( dataTable != null );
            Contract.Requires( dataRows != null );
            Contract.Requires( textWriter != null );

            DataColumnCollection columns = dataTable.Columns;

            if (columns.Count > 0)
            {
                var sb = new StringBuilder();

                foreach (DataColumn column in columns)
                {
                    sb.Append( column.ColumnName );
                    sb.Append( '\t' );
                }

                textWriter.WriteLine( sb );

                foreach (DataRow row in dataRows)
                {
                    sb.Length = 0;
                    Object[] itemArray = row.ItemArray;
                    Int32 last = itemArray.Length - 1;

                    for (Int32 i = 0; i < last; i++)
                    {
                        sb.Append( itemArray[ i ] );
                        sb.Append( '\t' );
                    }

                    sb.Append( itemArray[ last ] );
                    textWriter.WriteLine( sb );
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
        public static TResult GetValue<TResult>( Object value, Object inputNullValue, TResult outputNullValue )
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
        public static TResult GetValue<TResult>( Object value, TResult outputNullValue )
        {
            Object inputNullValue = DBNull.Value;
            return GetValue( value, inputNullValue, outputNullValue );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TResult GetValueOrDefault<TResult>( Object value )
        {
            Object inputNullValue = DBNull.Value;
            TResult outputNullValue = default( TResult );
            return GetValue( value, inputNullValue, outputNullValue );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataView"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static String ToString(
            DataView dataView,
            DataColumn[] columns )
        {
            Contract.Requires( dataView != null );

            String s = null;
            Int32 count = dataView.Count;

            if (count > 0)
            {
                Int32 columnCount = columns.Length;
                StringTable st = new StringTable( columnCount );
                DataTableExtensions.SetAlign( columns, st.Columns );
                DataTableExtensions.WriteHeader( columns, st );

                for (Int32 i = 0; i < count; i++)
                {
                    DataRow dataRow = dataView[ i ].Row;
                    StringTableRow row = st.NewRow();

                    for (Int32 j = 0; j < columns.Length; j++)
                    {
                        row[ j ] = dataRow[ columns[ j ] ].ToString();
                    }

                    st.Rows.Add( row );
                }

                s = st.ToString();
            }

            return s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="columnNumber"></param>
        /// <param name="rowNumber"></param>
        /// <returns></returns>
        public static String ToString(
            DataTable dataTable,
            Int32 columnNumber,
            Int32 rowNumber )
        {
            Contract.Requires( dataTable != null );

            DataColumn dataColumn = dataTable.Columns[ columnNumber ];
            Type type = dataColumn.DataType;
            TypeCode typeCode = Type.GetTypeCode( type );
            Object value = dataTable.Rows[ rowNumber ][ columnNumber ];
            String s;

            if (value == DBNull.Value)
            {
                s = "is null";
            }
            else
            {
                switch (typeCode)
                {
                    case TypeCode.String:
                        s = value.ToString().Replace( "\'", "''" );
                        s = String.Format( CultureInfo.InvariantCulture, "'{0}'", s );
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
            this.connection.Open();
        }

        /// <summary>
        /// Closes the underlying <see cref="System.Data.IDbConnection"/>.
        /// </summary>
        public void Close()
        {
            this.connection.Close();
        }

        /// <summary>
        /// Disposes the underlying <see cref="System.Data.IDbConnection"/>
        /// </summary>
        public void Dispose()
        {
            this.connection.Dispose();
        }

        /// <summary>
        ///  Begins a database transaction.
        /// </summary>
        /// <returns></returns>
        public IDbTransaction BeginTransaction()
        {
            Contract.Requires( this.Connection != null );

            return this.connection.BeginTransaction();
        }

        /// <summary>
        /// Begins a database transaction with the specified System.Data.IsolationLevel value.
        /// </summary>
        /// <param name="il"></param>
        /// <returns></returns>
        public IDbTransaction BeginTransaction( IsolationLevel il )
        {
            Contract.Requires( this.Connection != null );

            return this.connection.BeginTransaction( il );
        }

        /// <summary>
        /// Creates and returns a <see cref="System.Data.IDbCommand"/> objects associated with the connection. 
        /// </summary>
        /// <remarks>
        /// Sets the CommandTimeout property.
        /// </remarks>
        /// <returns>
        /// A <see cref="System.Data.IDbCommand"/> Object associated with the connection.
        /// </returns>
        public IDbCommand CreateCommand()
        {
            Contract.Requires( this.Connection != null );

            this.command = this.connection.CreateCommand();
            this.command.Transaction = this.transaction;
            this.command.CommandTimeout = this.commandTimeout;
            return this.command;
        }

        /// <summary>
        /// Creates and returns a Command Object associated with the connection.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>A <see cref="System.Data.IDbCommand"/> Object associated with the connection.</returns>
        public IDbCommand CreateCommand( String commandText )
        {
            this.command = this.CreateCommand();
            this.command.CommandText = commandText;
            return this.command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        /// <returns>A Command Object associated with the connection.</returns>
        public IDbCommand CreateCommand( String commandText, CommandType commandType )
        {
            this.command = this.CreateCommand( commandText );
            this.command.CommandType = commandType;
            return this.command;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        public void DeriveParameters( IDbCommand command )
        {
            Contract.Requires( this.CommandBuilderHelper != null );

            this.commandBuilderHelper.DeriveParameters( command );
        }

        /// <summary>
        /// Executes an SQL statement against the Connection Object of a .NET Framework data provider, and returns the number of rows affected.
        /// </summary>
        /// <param name="command"></param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public Int32 ExecuteNonQuery( IDbCommand command )
        {
            Contract.Requires( command != null );

            this.command = command;
            this.rowCount = command.ExecuteNonQuery();
            return this.rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public Int32 ExecuteNonQuery( String commandText )
        {
            //this.command = this.CreateCommand(commandText);
            //this.rowCount = this.command.ExecuteNonQuery();
            //return this.rowCount;

            this.command = this.CreateCommand( commandText );
            try
            {
                this.rowCount = this.command.ExecuteNonQuery();
            }
            catch (Exception exception)
            {
                throw new DbCommandExecutionException( "Database.ExecuteNonQuery failed.", exception, this.command );
            }

            return this.rowCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public Object ExecuteScalar( IDbCommand command )
        {
            Contract.Requires( command != null );

            this.command = command;
            Object scalar = command.ExecuteScalar();
            return scalar;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public Object ExecuteScalar( String commandText )
        {
            this.command = this.CreateCommand( commandText );
            Object scalar = this.command.ExecuteScalar();
            return scalar;
        }

        /// <summary>
        /// Executes the commandText against the <see cref="Connection"/> and builds an <see cref="System.Data.IDataReader"/>.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public IDataReader ExecuteReader( String commandText )
        {
            this.command = this.CreateCommand( commandText );
            return this.command.ExecuteReader();
        }

        /// <summary>
        /// Executes the commandText against the Connection, and builds an <see cref="IDataReader"/> using one of the <see cref="CommandBehavior"/> values.
        /// </summary>
        /// <param name="commandText">The text command to execute.</param>
        /// <param name="behavior">One of the <see cref="CommandBehavior"/> values.</param>
        /// <returns>An <see cref="IDataReader"/> Object.</returns>
        public IDataReader ExecuteReader( String commandText, CommandBehavior behavior )
        {
            this.command = this.CreateCommand( commandText );
            return this.command.ExecuteReader( behavior );
        }

        /// <summary>
        /// Executes the command and fills a <see cref="System.Data.DataTable"/>.
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable( String commandText )
        {
            this.command = this.CreateCommand( commandText );
            return this.command.ExecuteDataTable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTable( IDbCommand command )
        {
            Contract.Requires( command != null );

            this.command = command;
            var dataTable = new DataTable();
            this.rowCount = command.Fill( dataTable );
            return dataTable;
        }

        /// <summary>
        /// Executes the command and fills a <see cref="System.Data.DataSet"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet( IDbCommand command )
        {
            this.command = command;
            var dataSet = new DataSet();
            dataSet.Locale = CultureInfo.InvariantCulture;
            this.rowCount = command.Fill( dataSet );
            return dataSet;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public DataSet ExecuteDataSet( String commandText )
        {
            this.command = this.CreateCommand( commandText );
            return this.ExecuteDataSet( this.command );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public XmlDocument ExecuteXmlDocument( IDbCommand command )
        {
            Contract.Requires( command != null );
            Contract.Requires( this.CommandHelper != null );

            return this.commandHelper.ExecuteXmlDocument( command );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <returns></returns>
        public XmlDocument ExecuteXmlDocument( String commandText )
        {
            this.command = this.CreateCommand( commandText );
            return this.ExecuteXmlDocument( this.command );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        public DataTable FillSchema( IDbCommand command, DataTable dataTable )
        {
            Contract.Requires( command != null );

            this.command = command;
            DataTable schemaTable;

            using (IDataReader dataReader = command.ExecuteReader( CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo ))
            {
                schemaTable = FillSchema( dataReader, dataTable );
            }

            return schemaTable;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public DataTable[] FillSchema( IDbCommand command, DataSet dataSet )
        {
            Contract.Requires( command != null );

            DataTable[] schemaTables;

            using (IDataReader dataReader = command.ExecuteReader( CommandBehavior.SchemaOnly | CommandBehavior.KeyInfo ))
            {
                schemaTables = FillSchema( dataReader, dataSet );
            }

            return schemaTables;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandText"></param>
        /// <param name="dataSet"></param>
        /// <returns></returns>
        public Int32 Fill( String commandText, DataSet dataSet )
        {
            Contract.Requires( dataSet != null );

            this.command = this.CreateCommand( commandText );
            this.rowCount = this.command.Fill( dataSet );
            return this.rowCount;
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
            Int32 commandTimeout )
        {
            this.connection = connection;
            this.transaction = transaction;
            this.commandHelper = commandHelper;
            this.commandTimeout = commandTimeout;
        }

        #endregion

        #region Private Methods

        internal static void FillSchema( DataTable schemaTable, DataTable dataTable )
        {
            List<DataColumn> primaryKey = new List<DataColumn>();
            DataColumnCollection columns = dataTable.Columns;
            DataColumn isKeyColumn = columns[ "IsKey" ];

            foreach (DataRow row in schemaTable.Rows)
            {
                String columnName = (String)row[ "ColumnName" ];
                Type dataType = (Type)row[ "DataType" ];
                Boolean isKey = isKeyColumn != null && row.Field<Boolean?>( isKeyColumn ) == true;
                String columnNameAdd = columnName;
                Int32 index = 2;

                while (true)
                {
                    if (columns.Contains( columnNameAdd ))
                    {
                        columnNameAdd = String.Format( "{0}{1}", columnName, index );
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }

                DataColumn column = new DataColumn( columnNameAdd, dataType );
                columns.Add( column );

                if (isKey)
                {
                    primaryKey.Add( column );
                }
            }

            DataColumn[] array = primaryKey.ToArray();
            dataTable.PrimaryKey = array;
        }

        private static DataTable FillSchema(
            IDataReader dataReader,
            DataTable dataTable )
        {
            DataTable schemaTable = dataReader.GetSchemaTable();
            FillSchema( schemaTable, dataTable );
            return schemaTable;
        }

        private static DataTable[] FillSchema(
            IDataReader dataReader,
            DataSet dataSet )
        {
            WorkerThread thread = WorkerThread.Current;
            var schemaTables = new List<DataTable>();

            while (!thread.IsStopRequested)
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.InvariantCulture;
                DataTable schemaTable = FillSchema( dataReader, dataTable );
                dataSet.Tables.Add( dataTable );
                schemaTables.Add( schemaTable );

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