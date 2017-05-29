namespace DataCommander.Providers.SqlServer.FieldReader
{
    using System;
    using System.Data;
    using System.Globalization;
    using Field;

    sealed class MoneyDataFieldReader : IDataFieldReader
    {
        private static readonly NumberFormatInfo NumberFormatInfo;
        private readonly IDataRecord _dataRecord;
        private readonly int _columnOrdinal;

        static MoneyDataFieldReader()
        {
            NumberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            //numberFormatInfo.CurrencySymbol = string.Empty;
            //numberFormatInfo.CurrencyDecimalSeparator = ".";
            //numberFormatInfo.CurrencyGroupSeparator = ",";
            //numberFormatInfo.CurrencyGroupSizes = new int[] { 3, 3, 3, 3, 3, 3, 3 };
            //numberFormatInfo.CurrencyDecimalDigits = 6;
            
            NumberFormatInfo.NumberDecimalSeparator = ".";
            NumberFormatInfo.NumberGroupSeparator = ",";
            NumberFormatInfo.NumberGroupSizes = new int[] { 3 };
            NumberFormatInfo.NumberDecimalDigits = 4; 
        }

        public MoneyDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            _dataRecord = dataRecord;
            _columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (_dataRecord.IsDBNull(_columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    var d = _dataRecord.GetDecimal(_columnOrdinal);
                    value = new DecimalField(NumberFormatInfo, d, null);
                }

                return value;
            }
        }
    }
}