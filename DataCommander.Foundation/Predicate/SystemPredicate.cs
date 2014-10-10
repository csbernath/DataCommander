namespace DataCommander.Foundation
{
    using System;

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
            this.predicate = predicate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Boolean Evaluate(T value)
        {
            return this.predicate(value);
        }
    }
}