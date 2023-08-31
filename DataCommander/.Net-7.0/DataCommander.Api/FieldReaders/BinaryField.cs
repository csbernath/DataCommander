using System;
using System.Text;
using Foundation.Text;

namespace DataCommander.Api.FieldReaders;

public sealed class BinaryField : IConvertible
{
    private readonly string _stringValue;

    public BinaryField(byte[] bytes)
    {
        Value = bytes;
        var length = Math.Min(bytes.Length, 16);
        var chars = Hex.Encode(bytes, length, true);

        var stringBuilder = new StringBuilder();
        stringBuilder.Append("0x");
        stringBuilder.Append(chars);

        if (length < bytes.Length)
        {
            stringBuilder.Append(" (");
            stringBuilder.Append(bytes.Length);
            stringBuilder.Append(')');
        }

        _stringValue = stringBuilder.ToString();
    }

    public override string ToString() => _stringValue;

    public byte[] Value { get; }

    #region IConvertible Members

    TypeCode IConvertible.GetTypeCode() => throw new NotImplementedException();
    bool IConvertible.ToBoolean(IFormatProvider? provider) => throw new NotImplementedException();
    byte IConvertible.ToByte(IFormatProvider? provider) => throw new NotImplementedException();
    char IConvertible.ToChar(IFormatProvider? provider) => throw new NotImplementedException();
    DateTime IConvertible.ToDateTime(IFormatProvider? provider) => throw new NotImplementedException();
    decimal IConvertible.ToDecimal(IFormatProvider? provider) => throw new NotImplementedException();
    double IConvertible.ToDouble(IFormatProvider? provider) => throw new NotImplementedException();
    short IConvertible.ToInt16(IFormatProvider? provider) => throw new NotImplementedException();
    int IConvertible.ToInt32(IFormatProvider? provider) => throw new NotImplementedException();
    long IConvertible.ToInt64(IFormatProvider? provider) => throw new NotImplementedException();
    sbyte IConvertible.ToSByte(IFormatProvider? provider) => throw new NotImplementedException();
    float IConvertible.ToSingle(IFormatProvider? provider) => throw new NotImplementedException();
    string IConvertible.ToString(IFormatProvider? provider) => Hex.GetString(Value, true);
    object IConvertible.ToType(Type conversionType, IFormatProvider? provider) => throw new NotImplementedException();
    ushort IConvertible.ToUInt16(IFormatProvider? provider) => throw new NotImplementedException();
    uint IConvertible.ToUInt32(IFormatProvider? provider) => throw new NotImplementedException();
    ulong IConvertible.ToUInt64(IFormatProvider? provider) => throw new NotImplementedException();

    #endregion
}