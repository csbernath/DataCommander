namespace DataCommander.Foundation.Diagnostics
{
    using System;

    /// <summary>
    /// 
    /// </summary>
    internal sealed class IsErrorTypePredicate
    {
        private readonly ErrorType type;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public IsErrorTypePredicate( ErrorType type )
        {
            this.type = type;
        }

        public Boolean IsTrue( Error value )
        {
            return value.Type == this.type;
        }
    }
}