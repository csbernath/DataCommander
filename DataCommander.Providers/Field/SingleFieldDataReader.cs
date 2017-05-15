namespace DataCommander.Providers.Field
{
    using System;
    using System.Data;

    public sealed class SingleFieldDataReader : IDataFieldReader
    {
        private readonly IDataRecord dataRecord;
        private readonly int columnOrdinal;

        public SingleFieldDataReader(IDataRecord dataRecord, int columnOrdinal)
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
                    var singleValue = (float)this.dataRecord[this.columnOrdinal];
                    value = new SingleField(singleValue);
                }

                return value;
            }
        }

        #endregion
    }
}