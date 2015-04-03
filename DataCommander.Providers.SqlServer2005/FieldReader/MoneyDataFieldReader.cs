namespace DataCommander.Providers.SqlServer2005
{
    using System;
    using System.Data;
    using System.Globalization;

    sealed class MoneyDataFieldReader : IDataFieldReader
    {
        private static readonly NumberFormatInfo numberFormatInfo;
        private readonly IDataRecord dataRecord;
        private readonly int columnOrdinal;

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

                if (this.dataRecord.IsDBNull(this.columnOrdinal))
                {
                    value = DBNull.Value;
                }
                else
                {
                    decimal d = this.dataRecord.GetDecimal(this.columnOrdinal);
                    value = new DecimalField(numberFormatInfo, d, null);
                }

                return value;
            }
        }
    }
}