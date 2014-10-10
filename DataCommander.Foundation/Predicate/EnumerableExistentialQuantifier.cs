namespace DataCommander.Foundation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// 
    /// </summary>
    public sealed class EnumerableExistentialQuantifier<T> : PredicateClass<IEnumerable<T>>
    {
        private Func<T, Boolean> predicate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public EnumerableExistentialQuantifier( Func<T, Boolean> predicate )
        {
            Contract.Requires( predicate != null );

            this.predicate = predicate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Boolean Evaluate( IEnumerable<T> enumerable, Func<T, Boolean> predicate )
        {
            Contract.Requires( enumerable != null );
            Contract.Requires( predicate != null );

            return enumerable.Any( predicate );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Int32 IndexOf( IEnumerable<T> enumerable, Predicate<T> predicate )
        {
            Contract.Requires( enumerable != null );
            Contract.Requires( predicate != null );

            Int32 index = -1;

            foreach (T item in enumerable)
            {
                index++;

                if (predicate( item ))
                {
                    break;
                }
            }

            return index;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <returns></returns>
        public override Boolean Evaluate( IEnumerable<T> enumerable )
        {
            return Evaluate( enumerable, this.predicate );
        }
    }
}