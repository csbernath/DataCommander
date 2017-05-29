namespace Foundation.Linq
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public struct PreviousAndCurrent<T>
    {
        internal PreviousAndCurrent(T previous, T current)
        {
            this.Previous = previous;
            this.Current = current;
        }

        /// <summary>
        /// 
        /// </summary>
        public T Previous { get; }

        /// <summary>
        /// 
        /// </summary>
        public T Current { get; }
    }
}