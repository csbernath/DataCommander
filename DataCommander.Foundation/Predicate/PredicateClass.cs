namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PredicateClass<T>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static implicit operator Predicate<T>( PredicateClass<T> predicate )
        {
            return predicate.Evaluate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static PredicateClass<T> operator !( PredicateClass<T> predicate )
        {
            return new Negation<T>( predicate );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static PredicateClass<T> operator &( PredicateClass<T> x, PredicateClass<T> y )
        {
            return new Conjunction<T>( x, y );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static PredicateClass<T> operator |( PredicateClass<T> x, PredicateClass<T> y )
        {
            return new Disjunction<T>( x, y );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Boolean operator true( PredicateClass<T> x )
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Boolean operator false( PredicateClass<T> x )
        {
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public abstract Boolean Evaluate( T value );
    }
}