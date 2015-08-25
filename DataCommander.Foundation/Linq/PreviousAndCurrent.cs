namespace DataCommander.Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct PreviousAndCurrent<T>
    {
        private readonly T previous;
        private readonly T current;

        internal PreviousAndCurrent(T previous, T current)
        {
            this.previous = previous;
            this.current = current;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Previous
        {
            get
            {
                return this.previous;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public T Current
        {
            get
            {
                return this.current;
            }
        }
    }
}