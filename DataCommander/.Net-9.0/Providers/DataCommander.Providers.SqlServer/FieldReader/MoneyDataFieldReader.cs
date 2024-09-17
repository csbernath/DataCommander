using System;
using System.Data;
using System.Globalization;
using DataCommander.Api.FieldReaders;

namespace DataCommander.Providers.SqlServer.FieldReader;

internal sealed class MoneyDataFieldReader(
    IDataRecord dataRecord,
    int columnOrdinal) : IDataFieldReader
{
    private static readonly NumberFormatInfo NumberFormatInfo;

    static MoneyDataFieldReader()
    {
        NumberFormatInfo = (NumberFormatInfo) NumberFormatInfo.CurrentInfo.Clone();
        //numberFormatInfo.CurrencySymbol = string.Empty;
        //numberFormatInfo.CurrencyDecimalSeparator = ".";
        //numberFormatInfo.CurrencyGroupSeparator = ",";
        //numberFormatInfo.CurrencyGroupSizes = new int[] { 3, 3, 3, 3, 3, 3, 3 };
        //numberFormatInfo.CurrencyDecimalDigits = 6;

        NumberFormatInfo.NumberDecimalSeparator = ".";
        NumberFormatInfo.NumberGroupSeparator = ",";
        NumberFormatInfo.NumberGroupSizes = [3];
        NumberFormatInfo.NumberDecimalDigits = 4;
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
                value = new DecimalField(NumberFormatInfo, d, null);
            }

            return value;
        }
    }
}