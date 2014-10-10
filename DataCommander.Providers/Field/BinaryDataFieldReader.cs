namespace DataCommander.Providers
{
    using System;
    using System.Data;

    public sealed class BinaryDataFieldReader : IDataFieldReader
    {
        public BinaryDataFieldReader( IDataRecord dataRecord, int columnOrdinal )
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
                    long length = dataRecord.GetBytes( columnOrdinal, 0, null, 0, 0 );
                    byte[] buffer = new byte[ length ];
                    length = dataRecord.GetBytes( columnOrdinal, 0, buffer, 0, (int) length );
                    value = new BinaryField( buffer );
                }

                return value;
            }
        }

        IDataRecord dataRecord;
        int columnOrdinal;
    }
}
