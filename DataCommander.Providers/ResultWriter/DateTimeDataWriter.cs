using System;

namespace DataCommander.Providers
{
    internal sealed class DateTimeDataWriter : DataWriterBase
    {
        public override string ToString( object value )
        {
            string s;

            if (value == DBNull.Value)
            {
                s = new string( ' ', Width );
            }
            else
            {
                DateTimeField field = (DateTimeField) value;
                s = field.ToString().PadLeft( Width, ' ' );
            }

            return s;
        }
    }
}