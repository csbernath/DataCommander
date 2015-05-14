namespace DataCommander.Foundation
{
    using System;
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Negation<T> : PredicateClass<T>
    {
        private readonly PredicateClass<T> predicate;

        /// <summary>        
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        public Negation(PredicateClass<T> predicate)
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
            return !this.predicate.Evaluate(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "not (" + this.predicate + ")";
        }
    }
}