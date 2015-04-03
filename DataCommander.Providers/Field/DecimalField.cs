namespace DataCommander.Providers
{
    using System;
    using System.Globalization;

    /// <summary>
    /// Summary description for DecimalField.
    /// </summary>
    public sealed class DecimalField : IComparable
    {
        private readonly NumberFormatInfo numberFormatInfo;
        private readonly decimal decimalValue;
        private readonly string stringValue;

        public DecimalField(
            NumberFormatInfo numberFormatInfo,
            decimal decimalValue,
            string stringValue )
        {
            this.numberFormatInfo = numberFormatInfo;
            this.decimalValue = decimalValue;
            this.stringValue = stringValue;
        }

        public decimal DecimalValue
        {
            get
            {
                return this.decimalValue;
            }
        }

        public string StringValue
        {
            get
            {
                return this.stringValue;
            }
        }

        public override string ToString()
        {
            return this.decimalValue.ToString( "N", this.numberFormatInfo );
        }

        #region IComparable Members

        int IComparable.CompareTo( object obj )
        {
            var other = (DecimalField) obj;
            return this.decimalValue.CompareTo( other.decimalValue );
        }

        #endregion
    }
}