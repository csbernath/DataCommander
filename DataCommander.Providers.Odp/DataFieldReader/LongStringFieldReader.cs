namespace DataCommander.Providers.Odp
{
    using System;
    using System.Data;

    internal sealed class LongStringFieldReader : IDataFieldReader
    {
        private readonly IDataRecord dataRecord;
        private readonly int columnOrdinal;

        public LongStringFieldReader(
            IDataRecord dataRecord,
            int columnOrdinal )
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        object IDataFieldReader.Value
        {
            get
            {
                object value;

                if (dataRecord.IsDBNull( columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    string s = dataRecord.GetString( columnOrdinal );
                    value = new StringField( s, 1024 );
                }

                return value;
            }
        }
    }
}