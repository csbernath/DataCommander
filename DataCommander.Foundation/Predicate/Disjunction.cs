namespace DataCommander.Foundation
{
    using System.Diagnostics.Contracts;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class Disjunction<T> : PredicateClass<T>
    {
        private readonly PredicateClass<T> x;
        private readonly PredicateClass<T> y;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public Disjunction(PredicateClass<T> x, PredicateClass<T> y)
        {
            Contract.Requires(x != null);
            Contract.Requires(y != null);

            this.x = x;
            this.y = y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public override bool Evaluate(T value)
        {
            return this.x.Evaluate(value) || this.y.Evaluate(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "(" + this.x + " or " + this.y + ")";
        }
    }
}
