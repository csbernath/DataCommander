using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Foundation.Data.TextData;

public sealed class TextDataCommand : DbCommand
{
    private TextDataConnection? _connection;

    public new TextDataParameterCollection Parameters { get; } = [];

    [AllowNull]
    public override string CommandText { get; set; }

    public override int CommandTimeout { get; set; }

    public override CommandType CommandType { get; set; }

    public new TextDataReader ExecuteReader() => new(this, CommandBehavior.Default);

    public new TextDataReader ExecuteReader(CommandBehavior behavior) => new(this, behavior);

    public override void Cancel() => throw new NotImplementedException();

    protected override DbParameter CreateDbParameter() => throw new NotImplementedException();

    protected override DbConnection? DbConnection
    {
        get => _connection;
        set => _connection = (TextDataConnection?)value;
    }

    protected override DbParameterCollection DbParameterCollection => Parameters;

    protected override DbTransaction? DbTransaction
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override bool DesignTimeVisible
    {
        get => false;

        set => throw new NotImplementedException();
    }

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior) => new TextDataReader(this, behavior);

    public override int ExecuteNonQuery()
    {
        var columns = Parameters.GetParameterValue<IList<TextDataColumn>>("columns")!;
        var converters = Parameters.GetParameterValue<IList<ITextDataConverter>>("converters")!;
        var rows = Parameters.GetParameterValue<IEnumerable<object[]>>("rows")!;
        var getTextWriter = Parameters.GetParameterValue<IConverter<TextDataCommand, TextWriter>>("getTextWriter")!;
        var textWriter = getTextWriter.Convert(this);
        var textDataStreamWriter = new TextDataStreamWriter(textWriter, columns, converters);
        var count = 0;

        foreach (var row in rows)
        {
            textDataStreamWriter.WriteRow(row);
            count++;
        }

        return count;
    }

    public override object ExecuteScalar() => throw new NotImplementedException();

    public override void Prepare() => throw new NotImplementedException();

    public override UpdateRowSource UpdatedRowSource
    {
        get => throw new NotImplementedException();

        set => throw new NotImplementedException();
    }
}