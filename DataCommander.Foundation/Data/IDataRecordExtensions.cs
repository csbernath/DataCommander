namespace DataCommander.Foundation.Data
{
    using System;
    using System.Data;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class IDataRecordExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRecord"></param>
        /// <param name="ordinal"></param>
        /// <returns></returns>
        public static T GetValue<T>( this IDataRecord dataRecord, Int32 ordinal )
        {
            Contract.Requires( dataRecord != null );
            Object valueObject = dataRecord[ ordinal ];
            Contract.Assert( valueObject is T );

            return (T)valueObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRecord"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetValue<T>(
            this IDataRecord dataRecord,
            String name)
        {
            Contract.Requires( dataRecord != null );
            Object valueObject = dataRecord[ name ];
            Contract.Assert( valueObject is T );

            return (T)valueObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRecord"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>( this IDataRecord dataRecord, Int32 index )
        {
            Contract.Requires( dataRecord != null );

            Object value = dataRecord[ index ];
            return Database.GetValueOrDefault<T>( value );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataRecord"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static T GetValueOrDefault<T>(
            this IDataRecord dataRecord,
            String name)
        {
            Contract.Requires( dataRecord != null );
            Object value = dataRecord[ name ];
            return Database.GetValueOrDefault<T>( value );
        }
    }
}