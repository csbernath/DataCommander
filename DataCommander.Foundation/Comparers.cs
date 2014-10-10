#if FOUNDATION_3_5
namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public static class Comparers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static Boolean Equality<T>( IComparable<T> arg1, T arg2 )
        {
            Contract.Requires( arg1 != null );

            return arg1.CompareTo( arg2 ) == 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static Boolean GreaterThan<T>( IComparable<T> arg1, T arg2 )
        {
            Contract.Requires( arg1 != null );

            return arg1.CompareTo( arg2 ) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static Boolean GreaterThanOrEqual<T>( IComparable<T> arg1, T arg2 )
        {
            Contract.Requires( arg1 != null );

            return arg1.CompareTo( arg2 ) >= 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static Boolean Inequality<T>( IComparable<T> arg1, T arg2 )
        {
            Contract.Requires( arg1 != null );

            return arg1.CompareTo( arg2 ) != 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static Boolean LessThan<T>( IComparable<T> arg1, T arg2 )
        {
            Contract.Requires( arg1 != null );
            return arg1.CompareTo( arg2 ) < 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <returns></returns>
        public static Boolean LessThanOrEqual<T>( IComparable<T> arg1, T arg2 )
        {
            Contract.Requires( arg1 != null );

            return arg1.CompareTo( arg2 ) <= 0;
        }
    }
}
#endif