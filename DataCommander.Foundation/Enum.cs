namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class Enum<T>
        where T : struct
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T Parse( string value )
        {
            Contract.Requires( typeof(T).IsEnum );

            Type type = typeof(T);
            T t = (T)Enum.Parse( type, value );
            return t;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static T? ToNullableEnum( int? source )
        {
            Contract.Requires( typeof(T).IsEnum );

            T? target;
            if (source != null)
            {
                target = (T)Enum.ToObject( typeof(T), source.Value );
            }
            else
            {
                target = null;
            }
            return target;
        }
    }
}