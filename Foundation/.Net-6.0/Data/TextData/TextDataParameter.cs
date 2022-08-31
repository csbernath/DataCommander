using System;
using System.Data;
using System.Data.Common;

namespace Foundation.Data.TextData;

public sealed class TextDataParameter : DbParameter
{
    private string _name;
    private object _value;

    public TextDataParameter(string name, object value)
    {
        _name = name;
        _value = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public override DbType DbType
    {
        get => throw new NotImplementedException();

        set => throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public override ParameterDirection Direction
    {
        get => throw new NotImplementedException();

        set => throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public override bool IsNullable
    {
        get => throw new NotImplementedException();

        set => throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public override string ParameterName
    {
        get => _name;

        set => _name = value;
    }

    /// <summary>
    /// 
    /// </summary>
    public override void ResetDbType()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 
    /// </summary>
    public override int Size
    {
        get => throw new NotImplementedException();

        set => throw new NotImplementedException();
    }

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

    public override object Value
    {
        get => _value;

        set => _value = value;
    }
}