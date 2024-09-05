using System.Data;

namespace DataCommander.Api;

public abstract class DataParameterBase(IDataParameter parameter, int size, byte precision, byte scale)
{
    public DbType DbType
    {
        get => parameter.DbType;
        set => parameter.DbType = value;
    }

    protected abstract void SetSize(int size);

    public int Size
    {
        get => size;
        set => SetSize(value);
    }

    public byte Precision { get; } = precision;
    public byte Scale { get; } = scale;
}