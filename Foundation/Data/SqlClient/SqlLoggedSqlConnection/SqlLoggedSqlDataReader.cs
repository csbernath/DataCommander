using System;
using System.Data;
using System.Diagnostics;
using Foundation.Diagnostics;

namespace Foundation.Data.SqlClient.SqlLoggedSqlConnection
{
    internal sealed class SqlLoggedSqlDataReader : IDataReader
    {
        private readonly SqlLoggedSqlConnection _connection;
        private readonly IDbCommand _command;
        private bool _contains;
        private IDataReader _reader;
        private DateTime _startDate;
        private long _startTick;
        private bool _logged;

        public SqlLoggedSqlDataReader(
            SqlLoggedSqlConnection connection,
            IDbCommand command)
        {
            this._connection = connection;
            this._command = command;
        }

        public IDataReader Execute()
        {
            Exception exception = null;
            this._startDate = LocalTime.Default.Now;
            this._startTick = Stopwatch.GetTimestamp();

            try
            {
                this._reader = this._command.ExecuteReader();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var ticks = Stopwatch.GetTimestamp() - this._startTick;
                var duration = StopwatchTimeSpan.ToInt32(ticks, 1000);
                var filter = this._connection.Filter;
                this._contains = exception != null || filter == null ||
                                filter.Contains(this._connection.UserName, this._connection.HostName, this._command);

                if (this._contains)
                {
                    this._connection.CommandExeucte(this._command, this._startDate, duration, exception);
                    this._logged = true;
                }
            }

            return this;
        }

        public IDataReader Execute(CommandBehavior behavior)
        {
            Exception exception = null;
            this._startDate = LocalTime.Default.Now;
            this._startTick = Stopwatch.GetTimestamp();

            try
            {
                this._reader = this._command.ExecuteReader(behavior);
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var ticks = Stopwatch.GetTimestamp() - this._startTick;
                var duration = StopwatchTimeSpan.ToInt32(ticks, 1000);
                var filter = this._connection.Filter;
                this._contains = exception != null || filter == null ||
                                filter.Contains(this._connection.UserName, this._connection.HostName, this._command);

                if (this._contains)
                {
                    this._connection.CommandExeucte(this._command, this._startDate, duration, exception);
                    this._logged = true;
                }
            }

            return this;
        }

        public void Dispose()
        {
            this._reader.Dispose();

            if (this._contains && !this._logged)
            {
                var duration = Stopwatch.GetTimestamp() - this._startTick;
                this._connection.CommandExeucte(this._command, this._startDate, duration, null);
            }
        }

        public bool GetBoolean(int i)
        {
            return this._reader.GetBoolean(i);
        }

        public byte GetByte(int i)
        {
            return this._reader.GetByte(i);
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        public char GetChar(int i)
        {
            return this._reader.GetChar(i);
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this._reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }

        public IDataReader GetData(int i)
        {
            return this._reader.GetData(i);
        }

        public string GetDataTypeName(int i)
        {
            return this._reader.GetDataTypeName(i);
        }

        public DateTime GetDateTime(int i)
        {
            return this._reader.GetDateTime(i);
        }

        public decimal GetDecimal(int i)
        {
            return this._reader.GetDecimal(i);
        }

        public double GetDouble(int i)
        {
            return this._reader.GetDouble(i);
        }

        public Type GetFieldType(int i)
        {
            return this._reader.GetFieldType(i);
        }

        public float GetFloat(int i)
        {
            return this._reader.GetFloat(i);
        }

        public Guid GetGuid(int i)
        {
            return this._reader.GetGuid(i);
        }

        public short GetInt16(int i)
        {
            return this._reader.GetInt16(i);
        }

        public int GetInt32(int i)
        {
            return this._reader.GetInt32(i);
        }

        public long GetInt64(int i)
        {
            return this._reader.GetInt64(i);
        }

        public string GetName(int i)
        {
            return this._reader.GetName(i);
        }

        public int GetOrdinal(string name)
        {
            return this._reader.GetOrdinal(name);
        }

        public string GetString(int i)
        {
            return this._reader.GetString(i);
        }

        public object GetValue(int i)
        {
            return this._reader.GetValue(i);
        }

        public int GetValues(object[] values)
        {
            return this._reader.GetValues(values);
        }

        public bool IsDBNull(int i)
        {
            return this._reader.IsDBNull(i);
        }

        public int FieldCount => this._reader.FieldCount;

        public object this[string name] => this._reader[name];

        public object this[int i] => this._reader[i];

        public void Close()
        {
            Exception exception = null;

            try
            {
                this._reader.Close();
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                var duration = Stopwatch.GetTimestamp() - this._startTick;
                this._contains |= exception != null;

                if (this._contains && !this._logged)
                {
                    this._connection.CommandExeucte(this._command, this._startDate, duration, null);
                    this._logged = true;
                }
            }
        }

        public DataTable GetSchemaTable()
        {
            return this._reader.GetSchemaTable();
        }

        public bool NextResult()
        {
            bool nextResult;

            if (this._contains)
            {
                try
                {
                    nextResult = this._reader.NextResult();
                }
                catch (Exception e)
                {
                    var duration = Stopwatch.GetTimestamp() - this._startTick;
                    this._connection.CommandExeucte(this._command, this._startDate, duration, e);
                    this._logged = true;
                    throw;
                }
            }
            else
            {
                nextResult = this._reader.NextResult();
            }

            return nextResult;
        }

        public bool Read()
        {
            var read = false;

            try
            {
                read = this._reader.Read();
            }
            catch (Exception e)
            {
                var duration = Stopwatch.GetTimestamp() - this._startTick;
                this._connection.CommandExeucte(this._command, this._startDate, duration, e);
                this._logged = true;
                throw;
            }

            return read;
        }

        public int Depth => this._reader.Depth;

        public bool IsClosed => this._reader.IsClosed;

        public int RecordsAffected => this._reader.RecordsAffected;
    }
}