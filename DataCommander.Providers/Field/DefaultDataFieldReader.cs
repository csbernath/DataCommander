namespace DataCommander.Providers
{
    using System;
    using System.Data;

    public sealed class DefaultDataFieldReader : IDataFieldReader
	{
		private readonly IDataRecord dataRecord;
		private readonly int columnOrdinal;

		public DefaultDataFieldReader( IDataRecord dataRecord, int columnOrdinal )
		{
			this.dataRecord = dataRecord;
			this.columnOrdinal = columnOrdinal;
		}

		object IDataFieldReader.Value
		{
			get
            {
				object value;

				try
				{
					value = this.dataRecord.GetValue(this.columnOrdinal);					
				}
				catch (Exception e)
				{
					var name = this.dataRecord.GetName(this.columnOrdinal);
					var dataTypeName = this.dataRecord.GetDataTypeName( this.columnOrdinal );
					string message = $"dataRecord.GetValue(columnordinal) failed. Column name: {name}, column dataTypeName: {dataTypeName}";
					throw new Exception(message, e);
				}

				return value;
            }
		}
	}
}