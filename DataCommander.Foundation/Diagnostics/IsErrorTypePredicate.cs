namespace DataCommander.Foundation.Diagnostics
{
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
        public IsErrorTypePredicate(ErrorType type)
        {
            this.type = type;
        }

        public bool IsTrue(Error value)
        {
            return value.Type == this.type;
        }
    }
}