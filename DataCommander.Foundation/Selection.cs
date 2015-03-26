namespace DataCommander.Foundation
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    public static class Selection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static ArgumentEqualsSelection<TArgument> CreateArgumentEqualsSelection<TArgument>(TArgument argument) where TArgument : IEquatable<TArgument>
        {
            return new ArgumentEqualsSelection<TArgument>(argument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TArgument"></typeparam>
        /// <param name="argument"></param>
        /// <returns></returns>
        public static ArgumentIsSelection<TArgument> CreateArgumentIsSelection<TArgument>(TArgument argument) where TArgument : class
        {
            return new ArgumentIsSelection<TArgument>(argument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeIsSelection CreateTypeIsSelection(Type type)
        {
            return new TypeIsSelection(type);
        }
    }
}