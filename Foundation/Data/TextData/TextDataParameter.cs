using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Foundation.Data.TextData;

public sealed class TextDataParameter(string name, object value) : DbParameter
{
    private string? _name = name;
    private object? _value = value;

    public override DbType DbType
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override ParameterDirection Direction
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override bool IsNullable
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    [AllowNull]
    public override string ParameterName
    {
        get => _name!;
        set => _name = value;
    }

    public override void ResetDbType() => throw new NotImplementedException();

    public override int Size
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    [AllowNull]
    public override string SourceColumn
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override bool SourceColumnNullMapping
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override DataRowVersion SourceVersion
    {
        get => throw new NotImplementedException();
        set => throw new NotImplementedException();
    }

    public override object? Value
    {
        get => _value;
        set => _value = value;
    }
}