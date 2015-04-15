namespace DataCommander.Providers.Msi
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Linq;
	using System.Text;

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

		int System.Collections.IList.Add( object value )
		{
			throw new NotImplementedException();
		}

		void System.Collections.IList.Clear()
		{
			throw new NotImplementedException();
		}

		bool System.Collections.IList.Contains( object value )
		{
			throw new NotImplementedException();
		}

		int System.Collections.IList.IndexOf( object value )
		{
			throw new NotImplementedException();
		}

		void System.Collections.IList.Insert( int index, object value )
		{
			throw new NotImplementedException();
		}

		bool System.Collections.IList.IsFixedSize
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		bool System.Collections.IList.IsReadOnly
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		void System.Collections.IList.Remove( object value )
		{
			throw new NotImplementedException();
		}

		void System.Collections.IList.RemoveAt( int index )
		{
			throw new NotImplementedException();
		}

		object System.Collections.IList.this[ int index ]
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

		void System.Collections.ICollection.CopyTo( Array array, int index )
		{
			throw new NotImplementedException();
		}

		int System.Collections.ICollection.Count
		{
			get
			{
				return this.parameters.Count;
			}
		}

		bool System.Collections.ICollection.IsSynchronized
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		object System.Collections.ICollection.SyncRoot
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		#endregion
	}
}