using System;
using System.Data;
using System.Diagnostics;
using Foundation.Core;

namespace Foundation.Data.SqlClient.SqlLoggedSqlConnection;

internal sealed class SqlLoggedSqlDataReader(
    SqlLoggedSqlConnection connection,
    IDbCommand command) : IDataReader
{
    private bool _contains;
    private IDataReader? _reader;
    private DateTime _startDate;
    private long _startTick;
    private bool _logged;

    public IDataReader Execute()
    {
        Exception? exception = null;
        _startDate = LocalTime.Default.Now;
        _startTick = Stopwatch.GetTimestamp();

        try
        {
            _reader = command.ExecuteReader();
        }
        catch (Exception e)
        {
            exception = e;
            throw;
        }
        finally
        {
            var ticks = Stopwatch.GetTimestamp() - _startTick;
            var duration = StopwatchTimeSpan.ToInt32(ticks, 1000);
            var filter = connection.Filter;
            _contains = exception != null || filter == null ||
                        filter.Contains(connection.UserName, connection.HostName, command);

            if (_contains)
            {
                connection.CommandExeucte(command, _startDate, duration, exception);
                _logged = true;
            }
        }

        return this;
    }

    public IDataReader Execute(CommandBehavior behavior)
    {
        Exception? exception = null;
        _startDate = LocalTime.Default.Now;
        _startTick = Stopwatch.GetTimestamp();

        try
        {
            _reader = command.ExecuteReader(behavior);
        }
        catch (Exception e)
        {
            exception = e;
            throw;
        }
        finally
        {
            var ticks = Stopwatch.GetTimestamp() - _startTick;
            var duration = StopwatchTimeSpan.ToInt32(ticks, 1000);
            var filter = connection.Filter;
            _contains = exception != null || filter == null ||
                        filter.Contains(connection.UserName, connection.HostName, command);

            if (_contains)
            {
                connection.CommandExeucte(command, _startDate, duration, exception);
                _logged = true;
            }
        }

        return this;
    }

    public void Dispose()
    {
        _reader!.Dispose();

        if (_contains && !_logged)
        {
            var duration = Stopwatch.GetTimestamp() - _startTick;
            connection.CommandExeucte(command, _startDate, duration, null);
        }
    }

    public bool GetBoolean(int i) => _reader!.GetBoolean(i);

    public byte GetByte(int i) => _reader!.GetByte(i);

    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) => _reader!.GetBytes(i, fieldOffset, buffer, bufferoffset, length);

    public char GetChar(int i) => _reader!.GetChar(i);

    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) => _reader!.GetChars(i, fieldoffset, buffer, bufferoffset, length);

    public IDataReader GetData(int i) => _reader!.GetData(i);

    public string GetDataTypeName(int i) => _reader!.GetDataTypeName(i);

    public DateTime GetDateTime(int i) => _reader!.GetDateTime(i);

    public decimal GetDecimal(int i) => _reader!.GetDecimal(i);

    public double GetDouble(int i) => _reader!.GetDouble(i);

    public Type GetFieldType(int i) => _reader!.GetFieldType(i);

    public float GetFloat(int i) => _reader!.GetFloat(i);

    public Guid GetGuid(int i) => _reader!.GetGuid(i);

    public short GetInt16(int i) => _reader!.GetInt16(i);

    public int GetInt32(int i) => _reader!.GetInt32(i);

    public long GetInt64(int i) => _reader!.GetInt64(i);

    public string GetName(int i) => _reader!.GetName(i);

    public int GetOrdinal(string name) => _reader!.GetOrdinal(name);

    public string GetString(int i) => _reader!.GetString(i);

    public object GetValue(int i) => _reader!.GetValue(i);

    public int GetValues(object[] values) => _reader!.GetValues(values);

    public bool IsDBNull(int i) => _reader!.IsDBNull(i);

    public int FieldCount => _reader!.FieldCount;

    public object this[string name] => _reader![name];

    public object this[int i] => _reader![i];

    public void Close()
    {
        Exception? exception = null;

        try
        {
            _reader!.Close();
        }
        catch (Exception e)
        {
            exception = e;
            throw;
        }
        finally
        {
            var duration = Stopwatch.GetTimestamp() - _startTick;
            _contains |= exception != null;

            if (_contains && !_logged)
            {
                connection.CommandExeucte(command, _startDate, duration, null);
                _logged = true;
            }
        }
    }

    public DataTable? GetSchemaTable() => _reader!.GetSchemaTable();

    public bool NextResult()
    {
        bool nextResult;

        if (_contains)
        {
            try
            {
                nextResult = _reader!.NextResult();
            }
            catch (Exception e)
            {
                var duration = Stopwatch.GetTimestamp() - _startTick;
                connection.CommandExeucte(command, _startDate, duration, e);
                _logged = true;
                throw;
            }
        }
        else
        {
            nextResult = _reader!.NextResult();
        }

        return nextResult;
    }

    public bool Read()
    {
        bool read;

        try
        {
            read = _reader!.Read();
        }
        catch (Exception e)
        {
            var duration = Stopwatch.GetTimestamp() - _startTick;
            connection.CommandExeucte(command, _startDate, duration, e);
            _logged = true;
            throw;
        }

        return read;
    }

    public int Depth => _reader!.Depth;

    public bool IsClosed => _reader!.IsClosed;

    public int RecordsAffected => _reader!.RecordsAffected;
}