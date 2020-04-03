using System;
using DataCommander.Providers2.FieldNamespace;
using DataCommander.Providers2.ResultWriter;

namespace DataCommander.Providers.ResultWriter
{
    internal sealed class DateTimeDataWriter : DataWriterBase
    {
        public override string ToString(object value)
        {
            string result;
            if (value == DBNull.Value)
                result = new string(' ', Width);
            else
            {
                var field = (DateTimeField) value;
                result = field.ToString().PadLeft(Width, ' ');
            }

            return result;
        }
    }
}