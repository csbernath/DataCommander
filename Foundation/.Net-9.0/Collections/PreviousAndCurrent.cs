namespace Foundation.Collections;

public readonly struct PreviousAndCurrent<T>
{
    public readonly T Previous;
    public readonly T Current;

    internal PreviousAndCurrent(T previous, T current)
    {
        Previous = previous;
        Current = current;
    }
}