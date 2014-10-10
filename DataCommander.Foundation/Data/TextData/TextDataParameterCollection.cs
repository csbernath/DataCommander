namespace DataCommander.Foundation.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    public sealed class TextDataParameterCollection : DbParameterCollection, IList<TextDataParameter>
    {
        private IndexableCollection<TextDataParameter> collection;
        private ListIndex<TextDataParameter> listIndex;
        private UniqueIndex<String, TextDataParameter> nameIndex;

        /// <summary>
        /// 
        /// </summary>
        public TextDataParameterCollection()
        {
            this.listIndex = new ListIndex<TextDataParameter>( "List" );
            this.nameIndex = new UniqueIndex<String, TextDataParameter>(
                "Name",
                parameter => GetKeyResponse.Create( true, parameter.ParameterName ),
                SortOrder.None );

            this.collection = new IndexableCollection<TextDataParameter>( this.listIndex );
            this.collection.Indexes.Add( this.nameIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Int32 Add( Object value )
        {
            Contract.Requires( value != null );
            Contract.Requires( value is TextDataParameter );

            TextDataParameter parameter = (TextDataParameter)value;
            this.collection.Add( parameter );
            return this.collection.Count - 1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public TextDataParameter Add( TextDataParameter parameter )
        {
            Contract.Assert( parameter != null );

            this.collection.Add( parameter );
            return parameter;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="values"></param>
        public override void AddRange( Array values )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Clear()
        {
            this.collection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [Pure]
        public override Boolean Contains( String value )
        {
            return this.nameIndex.ContainsKey( value );
        }

#if FOUNDATION_3_5
#else
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
        [Pure]
        public Boolean PureContains(String value)
        {
            return this.Contains(value);
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
#if FOUNDATION_3_5
#else
        [Pure]
#endif
        public override Boolean Contains( Object value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public override void CopyTo( Array array, Int32 index )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Int32 Count
        {
            get
            {
                return this.collection.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override System.Collections.IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        protected override DbParameter GetParameter( String parameterName )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected override DbParameter GetParameter( Int32 index )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public override Int32 IndexOf( String parameterName )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Int32 IndexOf( Object value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        public override void Insert( Int32 index, Object value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean IsFixedSize
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean IsReadOnly
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Boolean IsSynchronized
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Remove( Object value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        public override void RemoveAt( String parameterName )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public override void RemoveAt( Int32 index )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="value"></param>
        protected override void SetParameter( String parameterName, DbParameter value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        protected override void SetParameter( Int32 index, DbParameter value )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        public override Object SyncRoot
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public TResult GetParameterValue<TResult>( String parameterName )
        {
            Contract.Assert( this.Contains( parameterName ) );
            TextDataParameter parameter = this.nameIndex[ parameterName ];
            Object value = parameter.Value;
            Contract.Assert( value is TResult );
            return (TResult)value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameterName"></param>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public Boolean TryGetValue( String parameterName, out TextDataParameter parameter )
        {
            return this.nameIndex.TryGetValue( parameterName, out parameter );
        }

        #region IList<TextDataParameter> Members

        Int32 IList<TextDataParameter>.IndexOf( TextDataParameter item )
        {
            Contract.Assert( item != null );

            return this.listIndex.IndexOf( item );
        }

        void IList<TextDataParameter>.Insert( Int32 index, TextDataParameter item )
        {
            throw new NotImplementedException();
        }

        void IList<TextDataParameter>.RemoveAt( Int32 index )
        {
            TextDataParameter parameter = this.listIndex[ index ];
            this.collection.Remove( parameter );
        }

        TextDataParameter IList<TextDataParameter>.this[ Int32 index ]
        {
            get
            {
                return this.listIndex[ index ];
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion

        #region ICollection<TextDataParameter> Members

        void ICollection<TextDataParameter>.Add( TextDataParameter item )
        {
            Contract.Assert( item != null );
            this.collection.Add( item );
        }

        void ICollection<TextDataParameter>.Clear()
        {
            this.collection.Clear();
        }

        Boolean ICollection<TextDataParameter>.Contains( TextDataParameter item )
        {
            throw new NotImplementedException();
        }

        void ICollection<TextDataParameter>.CopyTo( TextDataParameter[] array, Int32 arrayIndex )
        {
            throw new NotImplementedException();
        }

        Int32 ICollection<TextDataParameter>.Count
        {
            get
            {
                return this.collection.Count;
            }
        }

        Boolean ICollection<TextDataParameter>.IsReadOnly
        {
            get
            {
                return this.collection.IsReadOnly;
            }
        }

        Boolean ICollection<TextDataParameter>.Remove( TextDataParameter item )
        {
            Contract.Assert( item != null );
            return this.collection.Remove( item );
        }

        #endregion

        #region IEnumerable<TextDataParameter> Members

        IEnumerator<TextDataParameter> IEnumerable<TextDataParameter>.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.collection.GetEnumerator();
        }

        #endregion
    }
}