namespace DataCommander.Providers.Field
{
    using System;
    using System.Globalization;

    public sealed class DateTimeOffsetField : IComparable, IConvertible
    {
        public DateTimeOffsetField(DateTimeOffset value)
        {
            Value = value;
        }

        public DateTimeOffset Value { get; }

        private static string ToString(DateTimeOffset value)
        {
            //string format;

            //if (value.TimeOfDay.Ticks == 0)
            //{
            //    format = "yyyy-MM-ddZ";
            //}
            //else if (value.Date.Ticks == 0)
            //{
            //    format = "HH:mm:ss.fffZ";
            //}
            //else
            //{
            //    format = "yyyy-MM-dd HH:mm:ss.fffZ";
            //}

            //return value.ToString(format);

            // TODO
            return value.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return ToString(Value);
        }

        #region IComparable Members

        public int CompareTo(object obj)
        {
            // TODO
            return 0;
            //int result;
            //Type type = obj.GetType();
            //TypeCode typeCode = Type.GetTypeCode(type);

            //switch (typeCode)
            //{
            //    case TypeCode.String:
            //        string s = (string)obj;
            //        DateTime dateTime;
            //        bool succeeded = TryParse(s, out dateTime);

            //        if (succeeded)
            //        {
            //            result = this.value.CompareTo(dateTime);
            //        }
            //        else
            //        {
            //            result = -1;
            //        }

            //        break;

            //    case TypeCode.Object:
            //        DateTimeOffsetField dateTimeField = (DateTimeOffsetField)obj;
            //        result = this.value.CompareTo(dateTimeField.value);
            //        break;

            //    default:
            //        throw new NotImplementedException();
            //}

            //return result;
        }

        #endregion

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
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
            return Value.LocalDateTime;
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
            return ToString();
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