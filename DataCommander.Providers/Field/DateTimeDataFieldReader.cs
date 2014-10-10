namespace DataCommander.Providers
{
	using System;
	using System.Data;

	public sealed class DateTimeDataFieldReader : IDataFieldReader
	{
		private IDataRecord dataRecord;
		private int columnOrdinal;

		public DateTimeDataFieldReader(
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
					DateTime dateTime = dataRecord.GetDateTime( columnOrdinal );
					value = new DateTimeField( dateTime );
				}

				return value;
			}
		}
	}
}