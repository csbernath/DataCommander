using System.Data.SqlTypes;
using System.Diagnostics;

namespace Foundation.Data.SqlClient.PTypes;

public struct PXml
{
    private readonly SqlXml? _sqlXml;
    private readonly PValueType _type;

    public static readonly PXml Null = new(PValueType.Null);
    public static readonly PXml Default = new(PValueType.Default);
    public static readonly PXml Empty = new(PValueType.Empty);

    private PXml(PValueType type)
    {
        _type = type;
        _sqlXml = null;
    }

    public PXml(SqlXml value)
    {
        _sqlXml = value;

        _type = value != null ? PValueType.Value : PValueType.Null;
    }

    public readonly object? Value => _sqlXml;

    [DebuggerStepThrough]
    public static implicit operator PXml(SqlXml value) => new(value);
}