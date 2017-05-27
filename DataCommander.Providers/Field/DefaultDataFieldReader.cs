namespace DataCommander.Providers.Field
{
    using System;
    using System.Data;

    public sealed class DefaultDataFieldReader : IDataFieldReader
	{
		private readonly IDataRecord _dataRecord;
		private readonly int _columnOrdinal;

		public DefaultDataFieldReader( IDataRecord dataRecord, int columnOrdinal )
		{
			_dataRecord = dataRecord;
			_columnOrdinal = columnOrdinal;
		}

		object IDataFieldReader.Value
		{
			get
            {
				object value;

				try
				{
					value = _dataRecord.GetValue(_columnOrdinal);					
				}
				catch (Exception e)
				{
					var name = _dataRecord.GetName(_columnOrdinal);
					var dataTypeName = _dataRecord.GetDataTypeName( _columnOrdinal );
					var message = $"dataRecord.GetValue(columnordinal) failed. Column name: {name}, column dataTypeName: {dataTypeName}";
					throw new Exception(message, e);
				}

				return value;
            }
		}
	}
}