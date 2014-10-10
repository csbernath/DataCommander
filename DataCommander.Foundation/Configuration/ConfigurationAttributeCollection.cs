namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.Contracts;
    using DataCommander.Foundation.Collections;

    /// <summary>
    /// 
    /// </summary>
    [DebuggerTypeProxy( typeof(ConfigurationAttributeCollectionDebugger) )]
    [DebuggerDisplay( "Count = {Count}" )]
    public class ConfigurationAttributeCollection : IList<ConfigurationAttribute>
    {
        private IndexableCollection<ConfigurationAttribute> collection;
        private ListIndex<ConfigurationAttribute> listIndex;
        private UniqueIndex<String, ConfigurationAttribute> nameIndex;
        private String name;

        /// <summary>
        /// 
        /// </summary>
        public ConfigurationAttributeCollection()
        {
            this.listIndex = new ListIndex<ConfigurationAttribute>( "List" );

            this.nameIndex = new UniqueIndex<String, ConfigurationAttribute>(
                "NameIndex",
                attribute => GetKeyResponse.Create( true, attribute.Name ),
                SortOrder.None );

            this.collection = new IndexableCollection<ConfigurationAttribute>( this.listIndex );
            this.collection.Indexes.Add( this.nameIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ConfigurationAttribute this[ Int32 index ]
        {
            get
            {
                Contract.Assert( 0 <= index && index < this.Count );

                return this.listIndex[ index ];
            }

            set
            {
                ConfigurationAttribute originalItem = this.listIndex[ index ];
                ICollection<ConfigurationAttribute> collection = this.nameIndex;
                collection.Remove( originalItem );
                this.listIndex[ index ] = value;
                collection.Add( value );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ConfigurationAttribute this[ String name ]
        {
            get
            {
                Contract.Requires( this.ContainsKey( name ) );

                return this.nameIndex[ name ];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public String Name
        {
            get
            {
                return this.name;
            }

            set
            {
                this.name = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="description"></param>
        public void Add( String name, Object value, String description )
        {
            Contract.Requires( !this.ContainsKey( name ) );
            ConfigurationAttribute attribute = new ConfigurationAttribute( name, value, description );
            this.collection.Add( attribute );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [Pure]
        public Boolean ContainsKey( String name )
        {
            return this.nameIndex.ContainsKey( name );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Int32 IndexOf( ConfigurationAttribute item )
        {
            return this.listIndex.IndexOf( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert( Int32 index, ConfigurationAttribute item )
        {
            ICollection<ConfigurationAttribute> collection = this.nameIndex;
            collection.Add( item );
            this.listIndex.Insert( index, item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Boolean Remove( String name )
        {
            ConfigurationAttribute attribute;
            Boolean contains = this.nameIndex.TryGetValue( name, out attribute );
            Boolean succeeded;

            if (contains)
            {
                succeeded = this.collection.Remove( attribute );
            }
            else
            {
                succeeded = false;
            }

            return succeeded;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RemoveAt( Int32 index )
        {
            ConfigurationAttribute item = this.listIndex[ index ];
            this.listIndex.RemoveAt( index );
            ICollection<ConfigurationAttribute> collection = this.nameIndex;
            collection.Remove( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public Boolean TryGetValue( String name, out ConfigurationAttribute attribute )
        {
            return this.nameIndex.TryGetValue( name, out attribute );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetAttributeValue<T>( String name, out T value )
        {
            return this.TryGetAttributeValue<T>( name, default( T ), out value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetAttributeValue<T>( String name, T defaultValue, out T value )
        {
            ConfigurationAttribute attribute;
            Boolean contains = this.nameIndex.TryGetValue( name, out attribute );

            if (contains)
            {
                value = attribute.GetValue<T>();
            }
            else
            {
                value = defaultValue;
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetAttributeValue( String name, Object value )
        {
            ConfigurationAttribute attribute;
            Boolean contains = this.nameIndex.TryGetValue( name, out attribute );

            if (contains)
            {
                attribute.Value = value;
            }
            else
            {
                attribute = new ConfigurationAttribute( name, value, null );
                this.collection.Add( attribute );
            }
        }

        #region ICollection<Attribute> Members

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Add( ConfigurationAttribute item )
        {
            this.collection.Add( item );
        }

        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            this.collection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Contains( ConfigurationAttribute item )
        {
            return this.collection.Contains( item );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public void CopyTo( ConfigurationAttribute[] array, Int32 arrayIndex )
        {
            this.collection.CopyTo( array, arrayIndex );
        }

        /// <summary>
        /// 
        /// </summary>
        public Int32 Count
        {
            get
            {
                return this.collection.Count;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Boolean IsReadOnly
        {
            get
            {
                return this.collection.IsReadOnly;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public Boolean Remove( ConfigurationAttribute item )
        {
            return this.collection.Remove( item );
        }

        #endregion

        #region IEnumerable<Attribute> Members

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ConfigurationAttribute> GetEnumerator()
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