namespace DataCommander.Providers
{
    using System;
    using System.Text;
    using DataCommander.Foundation.Text;

    public sealed class BinaryField : IConvertible
    {
        private readonly byte[] bytes;
        private readonly string s;

        public BinaryField(byte[] bytes)
        {
            this.bytes = bytes;
            int length = Math.Min(bytes.Length, 16);
            char[] chars = Hex.Encode(bytes, length, true);

            var sb = new StringBuilder();
            sb.Append("0x");
            sb.Append(chars);

            if (length < bytes.Length)
            {
                sb.Append(" (");
                sb.Append(bytes.Length);
                sb.Append(')');
            }

            this.s = sb.ToString();
        }

        public override string ToString()
        {
            return this.s;
        }

        public byte[] Value
        {
            get
            {
                return this.bytes;
            }
        }

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            throw new NotImplementedException();
        }

        bool IConvertible.ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        double IConvertible.ToDouble(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString(IFormatProvider provider)
        {
            return Hex.GetString(this.bytes, true);
        }

        object IConvertible.ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}