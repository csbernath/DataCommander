namespace Foundation.Core
{
    public readonly struct MappingPair<TSource, TTarget>
    {
        public readonly TSource Source;
        public readonly TTarget Target;

        public MappingPair(TSource source, TTarget target)
        {
            Source = source;
            Target = target;
        }
    }
}