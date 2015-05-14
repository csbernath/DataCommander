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
        private readonly Func<T, bool> predicate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public EnumerableUniversalQuantifier(Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null);

            this.predicate = predicate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static bool Evaluate(IEnumerable<T> enumerable, Func<T, bool> predicate)
        {
            Contract.Requires<ArgumentNullException>(enumerable != null);
            Contract.Requires<ArgumentNullException>(predicate != null);

            bool isTrue = true;

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
        public override bool Evaluate(IEnumerable<T> value)
        {
            return Evaluate(value, this.predicate);
        }
    }
}