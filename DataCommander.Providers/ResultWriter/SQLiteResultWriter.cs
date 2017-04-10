namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.Data.SQLite;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using DataCommander.Foundation.Data;
    using DataCommander.Foundation.Text;

    internal sealed class SQLiteResultWriter : IResultWriter
    {
        private readonly TextWriter messageWriter;
        private SQLiteConnection connection;
        private SQLiteTransaction transaction;
        private string tableName;
        private SQLiteCommand insertCommand;

        public SQLiteResultWriter(TextWriter messageWriter, string tableName)
        {
            this.messageWriter = messageWriter;
            this.tableName = tableName;
        }

        #region IResultWriter Members

        void IResultWriter.Begin(IProvider provider)
        {
        }

        void IResultWriter.BeforeExecuteReader(AsyncDataAdapterCommand command)
        {
        }

        void IResultWriter.AfterExecuteReader(int fieldCount)
        {
            var fileName = Path.GetTempFileName() + ".sqlite";
            this.messageWriter.WriteLine(fileName);
            var sb = new SQLiteConnectionStringBuilder();
            sb.DataSource = fileName;
            sb.DateTimeFormat = SQLiteDateFormats.ISO8601;
            this.connection = new SQLiteConnection(sb.ConnectionString);
            this.connection.Open();
        }

        void IResultWriter.AfterCloseReader(int affectedRows)
        {
        }

        void IResultWriter.WriteTableBegin(DataTable schemaTable)
        {
            Trace.WriteLine(schemaTable.ToStringTableString());
            var sb = new StringBuilder();
            var schemaRows = schemaTable.Rows;
            var schemaRowCount = schemaRows.Count;
            string insertStatement = null;
            var insertValues = new StringBuilder();
            this.insertCommand = new SQLiteCommand();
            var st = new StringTable(3);

            for (var i = 0; i < schemaRowCount; i++)
            {
                var schemaRow = new DbColumn(schemaRows[i]);
                var stringTableRow = st.NewRow();

                if (i == 0)
                {
                    var tableName = schemaRow.BaseTableName;

                    if (tableName != null)
                    {
                        this.tableName = tableName;
                    }

                    this.tableName = this.tableName.Replace('.', '_');
                    this.tableName = this.tableName.Replace('[', '_');
                    this.tableName = this.tableName.Replace(']', '_');

                    sb.AppendFormat("CREATE TABLE {0}\r\n(\r\n", this.tableName);
                    insertStatement = "INSERT INTO " + this.tableName + '(';
                    insertValues.Append("VALUES(");
                }
                else
                {
                    insertStatement += ',';
                    insertValues.Append(',');
                }

                var columnName = schemaRow.ColumnName;
                stringTableRow[1] = columnName;
                insertStatement += columnName;
                insertValues.Append('?');
                var columnSize = (int)schemaRow.ColumnSize;
                var dataType = schemaRow.DataType;
                var typeCode = Type.GetTypeCode(dataType);
                string typeName;
                DbType dbType;

                switch (typeCode)
                {
                    case TypeCode.Boolean:
                        typeName = "BOOLEAN";
                        dbType = DbType.Boolean;
                        break;

                    case TypeCode.DateTime:
                        typeName = "DATETIME";
                        dbType = DbType.DateTime;
                        break;

                    case TypeCode.Decimal:
                        var precision = schemaRow.NumericPrecision.Value;
                        var scale = schemaRow.NumericPrecision.Value;

                        if (precision <= 28 && scale <= 28)
                        {
                            typeName = $"DECIMAL({precision},{scale})";
                            dbType = DbType.Decimal;
                        }
                        else
                        {
                            typeName = $"-- DECIMAL({precision},{scale})";
                            dbType = DbType.Object;
                        }

                        break;

                    case TypeCode.Double:
                        typeName = "DOUBLE";
                        dbType = DbType.Double;
                        break;

                    case TypeCode.Int16:
                        typeName = "SMALLINT";
                        dbType = DbType.Int16;
                        break;

                    case TypeCode.Int32:
                        typeName = "INT";
                        dbType = DbType.Int32;
                        break;

                    case TypeCode.Int64:
                        typeName = "BIGINT";
                        dbType = DbType.Int64;
                        break;

                    case TypeCode.String:
                        typeName = $"VARCHAR({columnSize})";
                        dbType = DbType.String;
                        break;

                    default:
                        throw new NotImplementedException();
                }

                stringTableRow[2] = typeName;
                st.Rows.Add(stringTableRow);
                var allowDBNull = schemaRow.AllowDBNull.Value;

                if (!allowDBNull)
                {
                    stringTableRow[2] += " NOT NULL";
                }

                if (i < schemaRowCount - 1)
                {
                    stringTableRow[2] += ',';
                }

                var parameter = new SQLiteParameter(dbType);
                this.insertCommand.Parameters.Add(parameter);
            }

            sb.Append(st.ToString(4));
            sb.Append(')');
            insertValues.Append(')');
            insertStatement += ") " + insertValues;
            var commandText = sb.ToString();
            Trace.WriteLine(commandText);
            Trace.WriteLine(insertStatement);
            var transactionScope = new DbTransactionScope(this.connection, null);
            transactionScope.ExecuteNonQuery(new CommandDefinition {CommandText = commandText});
            this.transaction = this.connection.BeginTransaction();
            this.insertCommand.Connection = this.connection;
            this.insertCommand.Transaction = this.transaction;
            this.insertCommand.CommandText = insertStatement;
        }

        void IResultWriter.FirstRowReadBegin()
        {
        }

        void IResultWriter.FirstRowReadEnd(string[] dataTypeNames)
        {
        }

        void IResultWriter.WriteRows(object[][] rows, int rowCount)
        {
            for (var i = 0; i < rowCount; i++)
            {
                var row = rows[i];

                for (var j = 0; j < row.Length; j++)
                {
                    var parameter = this.insertCommand.Parameters[j];
                    parameter.Value = row[j];
                }

                this.insertCommand.ExecuteNonQuery();
            }
        }

        void IResultWriter.WriteTableEnd()
        {
        }

        void IResultWriter.WriteParameters(IDataParameterCollection parameters)
        {
        }

        void IResultWriter.End()
        {
            if (this.transaction != null)
            {
                this.transaction.Commit();
                this.transaction.Dispose();
                this.transaction = null;
            }

            if (this.connection != null)
            {
                this.connection.Close();
                this.connection.Dispose();
                this.connection = null;
            }
        }

        #endregion
    }
}