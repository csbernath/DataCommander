namespace DataCommander.Providers
{
	using System;
    using System.Data;
	using System.IO;

    public sealed class StreamFieldDataReader : IDataFieldReader
	{
		private IDataRecord dataRecord;
		private int columnOrdinal;

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
				Stream stream = (Stream) this.dataRecord[ columnOrdinal ];
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