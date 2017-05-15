namespace DataCommander.Providers.ResultWriter
{
    using System;

    internal sealed class BooleanDataWriter : DataWriterBase
    {
        public override string ToString( object value )
        {
            string s;

            if (value == DBNull.Value)
            {
                s = new string( ' ', this.Width );
            }
            else
            {
                s = value.ToString().PadLeft(this.Width );
            }

            return s;
        }
    }
}