namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class SystemPredicate<T> : PredicateClass<T>
    {
        private readonly Predicate<T> predicate;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public SystemPredicate(Predicate<T> predicate)
        {
            Contract.Requires<ArgumentNullException>(predicate != null);

            this.predicate = predicate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Evaluate(T value)
        {
            return this.predicate(value);
        }
    }
}