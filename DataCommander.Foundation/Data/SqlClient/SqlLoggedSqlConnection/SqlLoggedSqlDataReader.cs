namespace DataCommander.Foundation.Data.SqlClient
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using DataCommander.Foundation.Diagnostics;

    internal sealed class SqlLoggedSqlDataReader : IDataReader
    {
        private readonly SqlLoggedSqlConnection connection;
        private IDbCommand command;
        private Boolean contains;
        private IDataReader reader;
        private DateTime startDate;
        private Int64 startTick;
        private Boolean logged;

        public SqlLoggedSqlDataReader(
            SqlLoggedSqlConnection connection,
            IDbCommand command )
        {
            this.connection = connection;
            this.command = command;
        }

        public IDataReader Execute()
        {
            Exception exception = null;
            this.startDate = OptimizedDateTime.Now;
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
                Int64 ticks = Stopwatch.GetTimestamp() - this.startTick;
                Int32 duration = StopwatchTimeSpan.ToInt32( ticks, 1000 );
                ISqlLoggedSqlCommandFilter filter = this.connection.Filter;
                this.contains = exception != null || filter == null || filter.Contains( this.connection.UserName, this.connection.HostName, this.command );

                if (this.contains)
                {
                    this.connection.CommandExeucte( this.command, this.startDate, duration, exception );
                    this.logged = true;
                }
            }

            return this;
        }

        public IDataReader Execute( CommandBehavior behavior )
        {
            Exception exception = null;
            this.startDate = OptimizedDateTime.Now;
            this.startTick = Stopwatch.GetTimestamp();

            try
            {
                this.reader = this.command.ExecuteReader( behavior );
            }
            catch (Exception e)
            {
                exception = e;
                throw;
            }
            finally
            {
                Int64 ticks = Stopwatch.GetTimestamp() - this.startTick;
                Int32 duration = StopwatchTimeSpan.ToInt32( ticks, 1000 );
                ISqlLoggedSqlCommandFilter filter = this.connection.Filter;
                this.contains = exception != null || filter == null || filter.Contains( this.connection.UserName, this.connection.HostName, this.command );

                if (this.contains)
                {
                    this.connection.CommandExeucte( this.command, this.startDate, duration, exception );
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
                Int64 duration = Stopwatch.GetTimestamp() - this.startTick;
                this.connection.CommandExeucte( this.command, this.startDate, duration, null );
            }
        }

        public Boolean GetBoolean( Int32 i )
        {
            return this.reader.GetBoolean( i );
        }

        public Byte GetByte( Int32 i )
        {
            return this.reader.GetByte( i );
        }

        public Int64 GetBytes( Int32 i, Int64 fieldOffset, Byte[] buffer, Int32 bufferoffset, Int32 length )
        {
            return this.reader.GetBytes( i, fieldOffset, buffer, bufferoffset, length );
        }

        public Char GetChar( Int32 i )
        {
            return this.reader.GetChar( i );
        }

        public Int64 GetChars( Int32 i, Int64 fieldoffset, Char[] buffer, Int32 bufferoffset, Int32 length )
        {
            return this.reader.GetChars( i, fieldoffset, buffer, bufferoffset, length );
        }

        public IDataReader GetData( Int32 i )
        {
            return this.reader.GetData( i );
        }

        public String GetDataTypeName( Int32 i )
        {
            return this.reader.GetDataTypeName( i );
        }

        public DateTime GetDateTime( Int32 i )
        {
            return this.reader.GetDateTime( i );
        }

        public Decimal GetDecimal( Int32 i )
        {
            return this.reader.GetDecimal( i );
        }

        public Double GetDouble( Int32 i )
        {
            return this.reader.GetDouble( i );
        }

        public Type GetFieldType( Int32 i )
        {
            return this.reader.GetFieldType( i );
        }

        public Single GetFloat( Int32 i )
        {
            return this.reader.GetFloat( i );
        }

        public Guid GetGuid( Int32 i )
        {
            return this.reader.GetGuid( i );
        }

        public Int16 GetInt16( Int32 i )
        {
            return this.reader.GetInt16( i );
        }

        public Int32 GetInt32( Int32 i )
        {
            return this.reader.GetInt32( i );
        }

        public Int64 GetInt64( Int32 i )
        {
            return this.reader.GetInt64( i );
        }

        public String GetName( Int32 i )
        {
            return this.reader.GetName( i );
        }

        public Int32 GetOrdinal( String name )
        {
            return this.reader.GetOrdinal( name );
        }

        public String GetString( Int32 i )
        {
            return this.reader.GetString( i );
        }

        public Object GetValue( Int32 i )
        {
            return this.reader.GetValue( i );
        }

        public Int32 GetValues( Object[] values )
        {
            return this.reader.GetValues( values );
        }

        public Boolean IsDBNull( Int32 i )
        {
            return this.reader.IsDBNull( i );
        }

        public Int32 FieldCount
        {
            get
            {
                return this.reader.FieldCount;
            }
        }

        public Object this[ String name ]
        {
            get
            {
                return this.reader[ name ];
            }
        }

        public Object this[ Int32 i ]
        {
            get
            {
                return this.reader[ i ];
            }
        }

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
                Int64 duration = Stopwatch.GetTimestamp() - this.startTick;
                this.contains |= exception != null;

                if (this.contains && !this.logged)
                {
                    this.connection.CommandExeucte( this.command, this.startDate, duration, null );
                    this.logged = true;
                }
            }
        }

        public DataTable GetSchemaTable()
        {
            return this.reader.GetSchemaTable();
        }

        public Boolean NextResult()
        {
            Boolean nextResult = false;

            if (this.contains)
            {
                try
                {
                    nextResult = this.reader.NextResult();
                }
                catch (Exception e)
                {
                    Int64 duration = Stopwatch.GetTimestamp() - this.startTick;
                    this.connection.CommandExeucte( this.command, this.startDate, duration, e );
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

        public Boolean Read()
        {
            Boolean read = false;

            try
            {
                read = this.reader.Read();
            }
            catch (Exception e)
            {
                Int64 duration = Stopwatch.GetTimestamp() - this.startTick;
                this.connection.CommandExeucte( this.command, this.startDate, duration, e );
                this.logged = true;
                throw;
            }

            return read;
        }

        public Int32 Depth
        {
            get
            {
                return this.reader.Depth;
            }
        }

        public Boolean IsClosed
        {
            get
            {
                return this.reader.IsClosed;
            }
        }

        public Int32 RecordsAffected
        {
            get
            {
                return this.reader.RecordsAffected;
            }
        }
    }
}