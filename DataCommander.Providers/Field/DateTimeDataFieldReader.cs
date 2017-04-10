namespace DataCommander.Providers
{
    using System;
    using System.Data;

    public sealed class DateTimeDataFieldReader : IDataFieldReader
	{
		private readonly IDataRecord dataRecord;
		private readonly int columnOrdinal;

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

				if (this.dataRecord.IsDBNull(this.columnOrdinal ))
				{
					value = DBNull.Value;
				}
				else
				{
					var dateTime = this.dataRecord.GetDateTime(this.columnOrdinal );
					value = new DateTimeField( dateTime );
				}

				return value;
			}
		}
	}
}