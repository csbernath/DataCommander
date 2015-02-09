namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Data;
    using System.Globalization;

    sealed class MoneyDataFieldReader : IDataFieldReader
    {
        private static NumberFormatInfo numberFormatInfo;
        private IDataRecord dataRecord;
        private int columnOrdinal;

        static MoneyDataFieldReader()
        {
            numberFormatInfo = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            //numberFormatInfo.CurrencySymbol = string.Empty;
            //numberFormatInfo.CurrencyDecimalSeparator = ".";
            //numberFormatInfo.CurrencyGroupSeparator = ",";
            //numberFormatInfo.CurrencyGroupSizes = new int[] { 3, 3, 3, 3, 3, 3, 3 };
            //numberFormatInfo.CurrencyDecimalDigits = 6;
            
            numberFormatInfo.NumberDecimalSeparator = ".";
            numberFormatInfo.NumberGroupSeparator = ",";
            numberFormatInfo.NumberGroupSizes = new int[] { 3 };
            numberFormatInfo.NumberDecimalDigits = 4; 
        }

        public MoneyDataFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal)
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (dataRecord.IsDBNull(columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    decimal d = dataRecord.GetDecimal(columnOrdinal);
                    value = new DecimalField(numberFormatInfo, d, null);
                }

                return value;
            }
        }
    }
}