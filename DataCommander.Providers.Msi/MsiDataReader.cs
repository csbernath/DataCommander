namespace DataCommander.Providers.Msi
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using DataCommander.Foundation.Diagnostics;
	using Microsoft.Deployment.WindowsInstaller;
    using System.Diagnostics.Contracts;

	internal sealed class MsiDataReader : IDataReader
	{
		private MsiCommand command;
		private CommandBehavior behavior;
		private View view;
		private IEnumerator<Record> enumerator;
		private int recordsAffected;

		public MsiDataReader( MsiCommand command, CommandBehavior behavior )
		{
            Contract.Requires( command != null );
			this.command = command;
			this.behavior = behavior;
			this.view = this.command.Connection.Database.OpenView( this.command.CommandText );
		}

		public View View
		{
			get
			{
				return this.view;
			}
		}

		#region IDataReader Members

		void IDataReader.Close()
		{
			if (this.view != null)
			{
				this.view.Close();
			}
		}

		int IDataReader.Depth
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public DataTable GetSchemaTable()
		{
			DataTable table = new DataTable();
			DataColumnCollection columns = table.Columns;
			columns.Add( "ColumnName", typeof( string ) );
			columns.Add( "ColumnSize", typeof( int ) );
			columns.Add( "DataType", typeof( Type ) );
			columns.Add( "ProviderType", typeof( int ) );
			columns.Add( "Definition", typeof( string ) );

			foreach (ColumnInfo column in this.view.Columns)
			{
				table.Rows.Add( new object[]
				{
					column.Name,
					column.Size,
					column.Type,
					(int)column.DBType,
					column.ColumnDefinitionString
				} );
			}

			return table;
		}

		bool IDataReader.IsClosed
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IDataReader.NextResult()
		{
			return false;
		}

		bool IDataReader.Read()
		{
			if (this.enumerator == null)
			{
				this.view.Execute();
				this.enumerator = this.view.GetEnumerator();
			}

			bool read = this.enumerator.MoveNext();

			if (read)
			{
				this.recordsAffected++;
			}

			return read;
		}

		int IDataReader.RecordsAffected
		{
			get
			{
				return this.recordsAffected;
			}
		}

		#endregion

		#region IDisposable Members

		void IDisposable.Dispose()
		{
			if (this.enumerator != null)
			{
				this.enumerator.Dispose();
			}

			if (this.view != null)
			{
				this.view.Dispose();
			}
		}

		#endregion

		#region IDataRecord Members

		int IDataRecord.FieldCount
		{
			get
			{
				return this.view.Columns.Count;
			}
		}

		bool IDataRecord.GetBoolean( int i )
		{
			throw new NotImplementedException();
		}

		byte IDataRecord.GetByte( int i )
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetBytes( int i, long fieldOffset, byte[] buffer, int bufferoffset, int length )
		{
			throw new NotImplementedException();
		}

		char IDataRecord.GetChar( int i )
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetChars( int i, long fieldoffset, char[] buffer, int bufferoffset, int length )
		{
			throw new NotImplementedException();
		}

		IDataReader IDataRecord.GetData( int i )
		{
			throw new NotImplementedException();
		}

		string IDataRecord.GetDataTypeName( int i )
		{
            var column = this.view.Columns[ i ];
            DbType dbType = (DbType)column.DBType;
            string dataTypeName = dbType.ToString();
            return dataTypeName;
		}

		DateTime IDataRecord.GetDateTime( int i )
		{
			throw new NotImplementedException();
		}

		decimal IDataRecord.GetDecimal( int i )
		{
			throw new NotImplementedException();
		}

		double IDataRecord.GetDouble( int i )
		{
			throw new NotImplementedException();
		}

		Type IDataRecord.GetFieldType( int i )
		{
			throw new NotImplementedException();
		}

		float IDataRecord.GetFloat( int i )
		{
			throw new NotImplementedException();
		}

		Guid IDataRecord.GetGuid( int i )
		{
			throw new NotImplementedException();
		}

		short IDataRecord.GetInt16( int i )
		{
			throw new NotImplementedException();
		}

		int IDataRecord.GetInt32( int i )
		{
			throw new NotImplementedException();
		}

		long IDataRecord.GetInt64( int i )
		{
			throw new NotImplementedException();
		}

		string IDataRecord.GetName( int i )
		{
			throw new NotImplementedException();
		}

		int IDataRecord.GetOrdinal( string name )
		{
			throw new NotImplementedException();
		}

		string IDataRecord.GetString( int i )
		{
			throw new NotImplementedException();
		}

		public object GetValue( int i )
		{
			Record record = this.enumerator.Current;
			return record[ i + 1 ];
		}

		public int GetValues( object[] values )
		{
			int count;

			if (values != null)
			{
				Record record = this.enumerator.Current;
				count = Math.Min( values.Length, record.FieldCount );

				for (int i = 0; i < count; i++)
				{
					values[ i ] = record[ i + 1 ];
				}
			}
			else
			{
				count = 0;
			}

			return count;
		}

		bool IDataRecord.IsDBNull( int i )
		{
			throw new NotImplementedException();
		}

		object IDataRecord.this[ string name ]
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object IDataRecord.this[ int i ]
		{
			get
			{
				return this.GetValue( i );
			}
		}

		#endregion
	}
}