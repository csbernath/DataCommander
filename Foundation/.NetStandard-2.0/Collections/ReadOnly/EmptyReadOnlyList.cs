namespace Foundation.Collections.ReadOnly
{
    public static class EmptyReadOnlyList<T>
    {
        public static readonly ReadOnlyList<T> Value = new ReadOnlyList<T>(EmptyArray<T>.Value);
    }
}