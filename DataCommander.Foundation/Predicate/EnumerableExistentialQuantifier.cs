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
        private readonly Func<T, bool> predicate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public EnumerableExistentialQuantifier( Func<T, bool> predicate )
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
        public static bool Evaluate( IEnumerable<T> enumerable, Func<T, bool> predicate )
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
        public static int IndexOf( IEnumerable<T> enumerable, Predicate<T> predicate )
        {
            Contract.Requires( enumerable != null );
            Contract.Requires( predicate != null );

            int index = -1;

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
        public override bool Evaluate( IEnumerable<T> enumerable )
        {
            return Evaluate( enumerable, this.predicate );
        }
    }
}