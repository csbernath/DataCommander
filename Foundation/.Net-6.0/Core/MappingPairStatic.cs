namespace Foundation.Core;

public static class MappingPair
{
    public static MappingPair<TSource, TTarget> Create<TSource, TTarget>(TSource source, TTarget target) =>
        new(source, target);
}