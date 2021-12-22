namespace Foundation.Linq;

public sealed class LinerSearchResult<TSource, TResult>
{
    public readonly int Index;
    public readonly TSource Source;
    public readonly TResult Result;

    public LinerSearchResult(int index, TSource source, TResult result)
    {
        Index = index;
        Source = source;
        Result = result;
    }
}