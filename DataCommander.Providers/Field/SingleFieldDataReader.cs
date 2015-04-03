namespace DataCommander.Providers
{
    using System;
    using System.Data;

    public sealed class SingleFieldDataReader : IDataFieldReader
    {
        private readonly IDataRecord dataRecord;
        private readonly Int32 columnOrdinal;

        public SingleFieldDataReader(IDataRecord dataRecord, Int32 columnOrdinal)
        {
            this.dataRecord = dataRecord;
            this.columnOrdinal = columnOrdinal;
        }

        #region IDataFieldReader Members

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
                    Single singleValue = (Single)this.dataRecord[this.columnOrdinal];
                    value = new SingleField(singleValue);
                }

                return value;
            }
        }

        #endregion
    }
}