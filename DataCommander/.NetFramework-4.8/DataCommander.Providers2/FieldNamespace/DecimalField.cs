using System;
using System.Globalization;

namespace DataCommander.Providers2.FieldNamespace
{
    /// <summary>
    /// Summary description for DecimalField.
    /// </summary>
    public sealed class DecimalField : IComparable
    {
        private readonly NumberFormatInfo _numberFormatInfo;

        public DecimalField(
            NumberFormatInfo numberFormatInfo,
            decimal decimalValue,
            string stringValue)
        {
            _numberFormatInfo = numberFormatInfo;
            DecimalValue = decimalValue;
            StringValue = stringValue;
        }

        public decimal DecimalValue { get; }

        public string StringValue { get; }

        public override string ToString()
        {
            return DecimalValue.ToString("N", _numberFormatInfo);
        }

        #region IComparable Members

        int IComparable.CompareTo(object obj)
        {
            var other = (DecimalField) obj;
            return DecimalValue.CompareTo(other.DecimalValue);
        }

        #endregion
    }
}