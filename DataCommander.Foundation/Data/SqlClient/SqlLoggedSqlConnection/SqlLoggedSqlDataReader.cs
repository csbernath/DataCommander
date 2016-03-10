namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class SqlLoggedSqlDataReader : IDataReader
    {
        private readonly SqlLoggedSqlConnection connection;
        private readonly IDbCommand command;
        private bool contains;
        private IDataReader reader;
        private DateTime startDate;
        private long startTick;
        private bool logged;

        public SqlLoggedSqlDataReader(
            SqlLoggedSqlConnection connection,
            IDbCommand command)
        {
            this.connection = connection;
            this.command = command;
        }

        public IDataReader Execute()
        {
            Exception exception = null;
            this.startDate = LocalTime.Default.Now;
            this.startTick = Stopwatch.GetTimestamp();

            try
            {
                this.reader = this.command.ExecuteReader();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                long ticks = Stopwatch.GetTimestamp() - this.startTick;
                int duration = StopwatchTimeSpan.ToInt32(ticks, 1000);
                ISqlLoggedSqlCommandFilter filter = this.connection.Filter;
                this.contains = exception != null || filter == null ||
                                filter.Contains(this.connection.UserName, this.connection.HostName, this.command);

                if (this.contains)
                {
                    this.connection.CommandExeucte(this.command, this.startDate, duration, exception);
                    this.logged = true;
                }
            }

            return this;
        }

        public IDataReader Execute(CommandBehavior behavior)
        {
            Exception exception = null;
            this.startDate = LocalTime.Default.Now;
            this.startTick = Stopwatch.GetTimestamp();

            try
            {
                this.reader = this.command.ExecuteReader(behavior);
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                long ticks = Stopwatch.GetTimestamp() - this.startTick;
                int duration = StopwatchTimeSpan.ToInt32(ticks, 1000);
                ISqlLoggedSqlCommandFilter filter = this.connection.Filter;
                this.contains = exception != null || filter == null ||
                                filter.Contains(this.connection.UserName, this.connection.HostName, this.command);

                if (this.contains)
                {
                    this.connection.CommandExeucte(this.command, this.startDate, duration, exception);
                    this.logged = true;
                }
            }

            return this;
        }

        public void Dispose()
        {
            this.reader.Dispose();

            if (this.contains && !this.logged)
            {
                long duration = Stopwatch.GetTimestamp() - this.startTick;
                this.connection.CommandExeucte(this.command, this.startDate, duration, null);
            }
        }

        public bool GetBoolean(int i)
        {
            return this.reader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return this.reader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this.reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public Char GetChar(int i)
        {
            return this.reader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, Char[] buffer, int bufferoffset, int length)
        {
            return this.reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return this.reader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return this.reader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return this.reader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return this.reader.GetDecimal(i);
        }

        public Double GetDouble(int i)
        {
            return this.reader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return this.reader.GetFieldType(i);
        }

        public Single GetFloat(int i)
        {
            return this.reader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return this.reader.GetGuid(i);
        }

        public Int16 GetInt16(int i)
        {
            return this.reader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return this.reader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return this.reader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return this.reader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return this.reader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return this.reader.GetString(i);
        }

        public object GetValue(int i)
        {
            return this.reader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return this.reader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return this.reader.IsDBNull(i);
        }

        public int FieldCount => this.reader.FieldCount;

        public object this[string name] => this.reader[name];

        public object this[int i] => this.reader[i];

        public void Close()
        {
            Exception exception = null;

            try
            {
                this.reader.Close();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                long duration = Stopwatch.GetTimestamp() - this.startTick;
                this.contains |= exception != null;

                if (this.contains && !this.logged)
                {
                    this.connection.CommandExeucte(this.command, this.startDate, duration, null);
                    this.logged = true;
                }
            }
        }

        public DataTable GetSchemaTable()
        {
            return this.reader.GetSchemaTable();
        }

        public bool NextResult()
        {
            bool nextResult;

            if (this.contains)
            {
                try
                {
                    nextResult = this.reader.NextResult();
                }
                catch (Exception e)
                {
                    long duration = Stopwatch.GetTimestamp() - this.startTick;
                    this.connection.CommandExeucte(this.command, this.startDate, duration, e);
                    this.logged = true;
                    throw;
                }
            }
            else
            {
                nextResult = this.reader.NextResult();
            }

            return nextResult;
        }

        public bool Read()
        {
            bool read = false;

            try
            {
                read = this.reader.Read();
            }
            catch (Exception e)
            {
                long duration = Stopwatch.GetTimestamp() - this.startTick;
                this.connection.CommandExeucte(this.command, this.startDate, duration, e);
                this.logged = true;
                throw;
            }

            return read;
        }

        public int Depth => this.reader.Depth;

        public bool IsClosed => this.reader.IsClosed;

        public int RecordsAffected => this.reader.RecordsAffected;
    }
}