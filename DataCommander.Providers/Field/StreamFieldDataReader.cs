namespace DataCommander.Providers
{
    using System;
    using System.Data;
    using System.IO;

    public sealed class StreamFieldDataReader : IDataFieldReader
	{
		private readonly IDataRecord dataRecord;
		private readonly int columnOrdinal;

		public StreamFieldDataReader( IDataRecord dataRecord, int columnOrdinal )
		{
			this.dataRecord = dataRecord;
			this.columnOrdinal = columnOrdinal;
		}

		#region IDataFieldReader Members

		object IDataFieldReader.Value
		{
			get
			{
				var stream = (Stream) this.dataRecord[this.columnOrdinal ];
				object value;

				if (stream != null)
				{
					value = new StreamField( stream );
				}
				else
				{
					value = DBNull.Value;
				}

				return value;
			}
		}

		#endregion
	}
}