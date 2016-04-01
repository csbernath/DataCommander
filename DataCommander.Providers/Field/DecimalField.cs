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

        public DecimalField(
            NumberFormatInfo numberFormatInfo,
            decimal decimalValue,
            string stringValue )
        {
            this.numberFormatInfo = numberFormatInfo;
            this.DecimalValue = decimalValue;
            this.StringValue = stringValue;
        }

        public decimal DecimalValue { get; }

        public string StringValue { get; }

        public override string ToString()
        {
            return this.DecimalValue.ToString( "N", this.numberFormatInfo );
        }

        #region IComparable Members

        int IComparable.CompareTo( object obj )
        {
            var other = (DecimalField) obj;
            return this.DecimalValue.CompareTo( other.DecimalValue );
        }

        #endregion
    }
}