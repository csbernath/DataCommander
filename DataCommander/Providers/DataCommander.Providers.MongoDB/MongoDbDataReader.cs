using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace DataCommander.Providers.MongoDB;

public class MongoDbDataReader(MongoDbCommand command) : DbDataReader
{
    private IAsyncCursor<BsonDocument>? _asyncCursor;
    private IEnumerator<BsonDocument>? _enumerator;
    
    public override bool GetBoolean(int ordinal) => throw new NotImplementedException();

    public override byte GetByte(int ordinal) => throw new NotImplementedException();

    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => throw new NotImplementedException();

    public override char GetChar(int ordinal) => throw new NotImplementedException();

    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => throw new NotImplementedException();

    public override string GetDataTypeName(int ordinal)
    {
        return nameof(BsonDocument);
    }

    public override DateTime GetDateTime(int ordinal) => throw new NotImplementedException();

    public override decimal GetDecimal(int ordinal) => throw new NotImplementedException();

    public override double GetDouble(int ordinal) => throw new NotImplementedException();

    public override Type GetFieldType(int ordinal) => throw new NotImplementedException();

    public override float GetFloat(int ordinal) => throw new NotImplementedException();

    public override Guid GetGuid(int ordinal) => throw new NotImplementedException();

    public override short GetInt16(int ordinal) => throw new NotImplementedException();

    public override int GetInt32(int ordinal) => throw new NotImplementedException();

    public override long GetInt64(int ordinal) => throw new NotImplementedException();

    public override string GetName(int ordinal) => throw new NotImplementedException();

    public override int GetOrdinal(string name) => throw new NotImplementedException();

    public override string GetString(int ordinal) => throw new NotImplementedException();

    public override object GetValue(int ordinal) => throw new NotImplementedException();

    public override int GetValues(object[] values)
    {
        var jsonWriterSettings = new JsonWriterSettings
        {
            Indent = true,
            OutputMode = JsonOutputMode.RelaxedExtendedJson
        };
        values[0] = _enumerator!.Current.ToJson(jsonWriterSettings);
        return 1;
    }

    public override bool IsDBNull(int ordinal) => throw new NotImplementedException();

    public override int FieldCount => 1;

    public override object this[int ordinal] => throw new NotImplementedException();

    public override object this[string name] => throw new NotImplementedException();

    public override int RecordsAffected => -1;

    public override bool HasRows => throw new NotImplementedException();

    public override bool IsClosed => throw new NotImplementedException();

    public override bool NextResult() => false;

    public override bool Read() => throw new NotImplementedException();

    public override int Depth => throw new NotImplementedException();

    public override IEnumerator GetEnumerator() => throw new NotImplementedException();
    
    public override DataTable? GetSchemaTable()
    {
        var schemaTable = new DataTable();
        schemaTable.Columns.Add(SchemaTableColumn.ColumnName, typeof(string));
        schemaTable.Columns.Add(SchemaTableColumn.ColumnOrdinal, typeof(int));
        schemaTable.Columns.Add(SchemaTableColumn.ColumnSize, typeof(int));
        schemaTable.Columns.Add(SchemaTableColumn.IsUnique, typeof(bool));
        schemaTable.Columns.Add(SchemaTableColumn.IsKey, typeof(bool));
        schemaTable.Columns.Add(SchemaTableColumn.BaseColumnName, typeof(string));
        schemaTable.Columns.Add(SchemaTableColumn.BaseSchemaName, typeof(string));
        schemaTable.Columns.Add(SchemaTableColumn.BaseTableName, typeof(string));
        schemaTable.Columns.Add(SchemaTableColumn.DataType, typeof(Type));
        schemaTable.Columns.Add(SchemaTableColumn.AllowDBNull, typeof(bool));
        schemaTable.Columns.Add(SchemaTableColumn.ProviderType, typeof(int));
        schemaTable.Columns.Add(SchemaTableColumn.IsLong, typeof(bool));

        schemaTable.Rows.Add(["Document", 1, Int32.MaxValue, false, false, null, null, null, typeof(string), false, 0, false]);

        return schemaTable;
    }

    public override async Task<bool> ReadAsync(CancellationToken cancellationToken)
    {
        bool read;
        if (_enumerator != null)
        {
            var moveNext = _enumerator.MoveNext();
            if (moveNext)
                read = true;
            else
            {
                _enumerator.Dispose();
                _enumerator = null;
                read = await ReadFromAsyncCursor(cancellationToken);
            }
        }
        else
            read = await ReadFromAsyncCursor(cancellationToken);

        return read;
    }

    public async Task Execute(CancellationToken cancellationToken)
    {
        var mongoClient = new MongoClient(command.Connection.ConnectionStringAndCredential.ConnectionString);
        var database = mongoClient.GetDatabase(command.DatabaseName);
        var collection = database.GetCollection<BsonDocument>(command.CollectionName);
        FilterDefinition<BsonDocument> filter = command.CommandText;
        _asyncCursor = await collection.FindAsync(filter, null, cancellationToken);
    }

    private async Task<bool> ReadFromAsyncCursor(CancellationToken cancellationToken)
    {
        bool read;
        var moveNext = await _asyncCursor!.MoveNextAsync(cancellationToken);
        if (moveNext)
        {
            _enumerator = _asyncCursor.Current.GetEnumerator();
            moveNext = _enumerator.MoveNext();
            read = moveNext;
        }
        else
        {
            read = false;
        }
        return read;
    }
}