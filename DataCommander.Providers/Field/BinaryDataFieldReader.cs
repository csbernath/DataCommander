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

                if (this.dataRecord.IsDBNull(this.columnOrdinal ))
                {
                    value = DBNull.Value;
                }
                else
                {
                    var length = this.dataRecord.GetBytes(this.columnOrdinal, 0, null, 0, 0 );
                    var buffer = new byte[ length ];
                    length = this.dataRecord.GetBytes(this.columnOrdinal, 0, buffer, 0, (int) length );
                    value = new BinaryField( buffer );
                }

                return value;
            }
        }

        readonly IDataRecord dataRecord;
        readonly int columnOrdinal;
    }
}
