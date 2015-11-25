namespace DataCommander.Providers.Odp
{
    using System;
    using Oracle.ManagedDataAccess.Types;

    internal sealed class OracleDecimalField : IConvertible
    {
        private OracleDecimal oracleDecimal;

        public OracleDecimalField( OracleDecimal oracleDecimal )
        {
            this.oracleDecimal = oracleDecimal;
        }

        public override string ToString()
        {
            return this.oracleDecimal.ToString();
        }

        #region IConvertible Members

        TypeCode IConvertible.GetTypeCode()
        {
            return TypeCode.Object;
        }

        bool IConvertible.ToBoolean( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        byte IConvertible.ToByte( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        char IConvertible.ToChar( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        DateTime IConvertible.ToDateTime( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        decimal IConvertible.ToDecimal( IFormatProvider provider )
        {
            return this.oracleDecimal.Value;
        }

        double IConvertible.ToDouble( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        short IConvertible.ToInt16( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        int IConvertible.ToInt32( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        long IConvertible.ToInt64( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        sbyte IConvertible.ToSByte( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        float IConvertible.ToSingle( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        string IConvertible.ToString( IFormatProvider provider )
        {
            return this.oracleDecimal.ToString();
        }

        object IConvertible.ToType( Type conversionType, IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        ushort IConvertible.ToUInt16( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        uint IConvertible.ToUInt32( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        ulong IConvertible.ToUInt64( IFormatProvider provider )
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}