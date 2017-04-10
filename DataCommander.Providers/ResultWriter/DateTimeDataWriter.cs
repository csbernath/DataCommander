namespace DataCommander.Providers
{
    using System;

    internal sealed class DateTimeDataWriter : DataWriterBase
    {
        public override string ToString(object value)
        {
            string s;

            if (value == DBNull.Value)
            {
                s = new string(' ', this.Width);
            }
            else
            {
                var field = (DateTimeField) value;
                s = field.ToString().PadLeft(this.Width, ' ');
            }

            return s;
        }
    }
}