namespace Foundation.Collections;

public struct PreviousAndCurrent<T>
{
    public readonly T Previous;
    public readonly T Current;

    internal PreviousAndCurrent(T previous, T current)
    {
        Previous = previous;
        Current = current;
    }
}