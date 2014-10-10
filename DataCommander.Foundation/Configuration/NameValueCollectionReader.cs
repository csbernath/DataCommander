namespace DataCommander.Foundation.Configuration
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Globalization;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public delegate Boolean TryGetValue<TKey, TValue>( TKey key, out TValue value );

    /// <summary>
    /// 
    /// </summary>
    public class NameValueCollectionReader
    {
        private TryGetValue<String, String> tryGetValue;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="s"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public delegate Boolean TryParse<T>( String s, out T value );

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tryGetValue"></param>
        public NameValueCollectionReader( TryGetValue<String, String> tryGetValue )
        {
            Contract.Requires( tryGetValue != null );
            this.tryGetValue = tryGetValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="tryParse"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public T GetValue<T>( String name, TryParse<T> tryParse, T defaultValue )
        {
            T value;
            Boolean contains = this.TryGetValue<T>( name, tryParse, out value );

            if (!contains)
            {
                value = defaultValue;
            }

            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Boolean GetBoolean( String name, Boolean defaultValue )
        {
            return this.GetValue<Boolean>( name, Boolean.TryParse, defaultValue );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Double GetDouble( String name, Double defaultValue )
        {
            return this.GetValue<Double>( name, Double.TryParse, defaultValue );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public Int32 GetInt32( String name, Int32 defaultValue )
        {
            return this.GetValue<Int32>( name, Int32.TryParse, defaultValue );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public String GetString( String name )
        {
            String value;
            this.tryGetValue( name, out value );
            return value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetBoolean( String name, out Boolean value )
        {
            Boolean contains = this.TryGetValue<Boolean>( name, Boolean.TryParse, out value );
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="provider"></param>
        /// <param name="styles"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetDateTime( String name, IFormatProvider provider, DateTimeStyles styles, out DateTime value )
        {
            String s;
            Boolean contains = this.tryGetValue( name, out s );

            if (contains)
            {
                Boolean succeeded = DateTime.TryParse( s, provider, styles, out value );
                Contract.Assert( succeeded );
            }
            else
            {
                value = default( DateTime );
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetDouble( String name, out Double value )
        {
            String s;
            Boolean contains = this.tryGetValue( name, out s );

            if (contains)
            {
                Boolean succeeded = Double.TryParse( s, out value );
                Contract.Assert( succeeded );
            }
            else
            {
                value = default( Double );
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="style"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetDouble( String name, NumberStyles style, IFormatProvider provider, out Double value )
        {
            String s;
            Boolean contains = this.tryGetValue( name, out s );

            if (contains)
            {
                Boolean succeeded = Double.TryParse( s, style, provider, out value );
                Contract.Assert( succeeded );
            }
            else
            {
                value = default( Double );
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetEnum<T>( String name, out T value )
        {
            String s;
            Boolean contains = this.tryGetValue( name, out s );

            if (contains)
            {
                Type enumType = typeof(T);
                Object valueObject = Enum.Parse( enumType, s );
                value = (T)valueObject;
            }
            else
            {
                value = default( T );
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetInt16( String name, out Int16 value )
        {
            Boolean contains = this.TryGetValue<Int16>( name, Int16.TryParse, out value );
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetInt32( String name, out Int32 value )
        {
            Boolean contains = this.TryGetValue<Int32>( name, Int32.TryParse, out value );
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetInt64( String name, out Int64 value )
        {
            Boolean contains = this.TryGetValue<Int64>( name, Int64.TryParse, out value );
            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="style"></param>
        /// <param name="provider"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetSingle( String name, NumberStyles style, IFormatProvider provider, out Single value )
        {
            String s;
            Boolean contains = this.tryGetValue( name, out s );

            if (contains)
            {
                Boolean succeeded = Single.TryParse( s, style, provider, out value );
                Contract.Assert( succeeded );
            }
            else
            {
                value = default( Single );
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetString( String name, out String value )
        {
            String s;
            Boolean contains = this.tryGetValue( name, out s );

            if (contains)
            {
                value = s;
            }
            else
            {
                value = null;
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetTimeSpan( String name, out TimeSpan value )
        {
            String s;
            Boolean contains = this.tryGetValue( name, out s );

            if (contains)
            {
                value = TimeSpan.Parse( s );
            }
            else
            {
                value = default( TimeSpan );
            }

            return contains;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <param name="tryParse"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Boolean TryGetValue<T>( String name, TryParse<T> tryParse, out T value )
        {
            Contract.Requires( tryParse != null );
            String s;
            Boolean contains = this.tryGetValue( name, out s );

            if (contains)
            {
                Boolean succeeded = tryParse( s, out value );
                Contract.Assert( succeeded );
            }
            else
            {
                value = default( T );
            }

            return contains;
        }
    }
}