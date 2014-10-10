namespace DataCommander.Foundation
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    public sealed class EnumerableUniversalQuantifier<T> : PredicateClass<IEnumerable<T>>
    {
        private readonly Func<T, Boolean> predicate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public EnumerableUniversalQuantifier(Func<T, Boolean> predicate)
        {
            Contract.Requires(predicate != null);

            this.predicate = predicate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static Boolean Evaluate(IEnumerable<T> enumerable, Func<T, Boolean> predicate)
        {
            Contract.Requires(enumerable != null);
            Contract.Requires(predicate != null);

            Boolean isTrue = true;

            foreach (T item in enumerable)
            {
                if (!predicate(item))
                {
                    isTrue = false;
                    break;
                }
            }

            return isTrue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Boolean Evaluate(IEnumerable<T> value)
        {
            return Evaluate(value, this.predicate);
        }
    }
}