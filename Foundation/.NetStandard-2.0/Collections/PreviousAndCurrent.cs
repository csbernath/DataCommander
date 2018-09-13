namespace Foundation.Collections
{
    public struct PreviousAndCurrent<T>
    {
        internal PreviousAndCurrent(T previous, T current)
        {
            Previous = previous;
            Current = current;
        }

        public T Previous { get; }
        public T Current { get; }
    }
}