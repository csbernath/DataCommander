namespace DataCommander.Providers.Msi
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    internal sealed class MsiParameterCollection : IDataParameterCollection
	{
		private List<object> parameters = new List<object>();

		#region IDataParameterCollection Members

		bool IDataParameterCollection.Contains( string parameterName )
		{
			throw new NotImplementedException();
		}

		int IDataParameterCollection.IndexOf( string parameterName )
		{
			throw new NotImplementedException();
		}

		void IDataParameterCollection.RemoveAt( string parameterName )
		{
			throw new NotImplementedException();
		}

		object IDataParameterCollection.this[ string parameterName ]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IList Members

		int IList.Add( object value )
		{
			throw new NotImplementedException();
		}

		void IList.Clear()
		{
			throw new NotImplementedException();
		}

		bool IList.Contains( object value )
		{
			throw new NotImplementedException();
		}

		int IList.IndexOf( object value )
		{
			throw new NotImplementedException();
		}

		void IList.Insert( int index, object value )
		{
			throw new NotImplementedException();
		}

		bool IList.IsFixedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool IList.IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void IList.Remove( object value )
		{
			throw new NotImplementedException();
		}

		void IList.RemoveAt( int index )
		{
			throw new NotImplementedException();
		}

		object IList.this[ int index ]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region ICollection Members

		void ICollection.CopyTo( Array array, int index )
		{
			throw new NotImplementedException();
		}

		int ICollection.Count => this.parameters.Count;

        bool ICollection.IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}