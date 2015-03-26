namespace DataCommander.Foundation.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Globalization;
    using System.Reflection;

    /// <summary>
    /// 
    /// </summary>
    public static class EnumExtensions
    {
#if FOUNDATION_3_5
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static bool HasFlag( this Enum container, Enum flag )
        {
            Contract.Requires( container.GetType() == flag.GetType() );

            UInt64 containerUInt64 = Convert.ToUInt64( container );
            UInt64 flagUInt64 = Convert.ToUInt64( flag );
            bool hasFlag = ( containerUInt64 & flagUInt64 ) == flagUInt64;
            return hasFlag;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static T SetFlag<T>( this T container, T flag )
        {
            Contract.Requires( typeof(T).IsEnum );
            Type type = typeof(T);

            UInt64 containerUInt64 = Convert.ToUInt64( container, CultureInfo.InvariantCulture );
            UInt64 flagUInt64 = Convert.ToUInt64( flag, CultureInfo.InvariantCulture );
            containerUInt64 |= flagUInt64;
            return (T)Enum.ToObject( type, containerUInt64 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="flag"></param>
        /// <param name="set"></param>
        /// <returns></returns>
        public static T SetFlag<T>( this T container, T flag, bool set )
        {
            Contract.Requires( typeof(T).IsEnum );
            Type type = typeof(T);

            UInt64 containerUInt64 = Convert.ToUInt64( container );
            UInt64 flagUInt64 = Convert.ToUInt64( flag );

            if (set)
            {
                containerUInt64 |= flagUInt64;
            }
            else
            {
                containerUInt64 &= ~flagUInt64;
            }

            return (T)Enum.ToObject( type, containerUInt64 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="container"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static T ResetFlag<T>( this T container, T flag )
        {
            Contract.Requires( typeof(T).IsEnum );
            Type type = typeof(T);
            UInt64 containerUInt64 = Convert.ToUInt64( container );
            UInt64 flagUInt64 = Convert.ToUInt64( flag );
            containerUInt64 &= ~flagUInt64;
            return (T)Enum.ToObject( type, containerUInt64 );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<string, T>> GetPublicStaticFields<T>( Type type )
        {
            TypeCode typeCode = Type.GetTypeCode( typeof(T) );
            FieldInfo[] fields = type.GetFields( BindingFlags.Public | BindingFlags.Static );

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[ i ];
                string name = field.Name;
                TypeCode fieldTypeCode = Type.GetTypeCode( field.FieldType );

                if (fieldTypeCode == typeCode)
                {
                    T value = (T)field.GetValue( type );
                    yield return Tuple.Create( name, value );
                }
            }
        }
    }
}