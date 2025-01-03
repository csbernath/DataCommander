namespace Foundation.Linq;

public sealed class LinerSearchResult<TSource, TResult>(int index, TSource source, TResult result)
{
    public readonly int Index = index;
    public readonly TSource Source = source;
    public readonly TResult Result = result;
}