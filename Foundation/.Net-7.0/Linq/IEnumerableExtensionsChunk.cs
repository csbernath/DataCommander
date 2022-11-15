using System;
using System.Collections.Generic;
using Foundation.Assertions;

namespace Foundation.Linq;

public static partial class IEnumerableExtensions
{
    public static IEnumerable<TSource[]> Chunk<TSource>(this IEnumerable<TSource> source, int size)
    {
        ArgumentNullException.ThrowIfNull(source);
        Assert.IsTrue(size >= 1);

        return ChunkIterator(source, size);
    }

    private static IEnumerable<TSource[]> ChunkIterator<TSource>(IEnumerable<TSource> source, int size)
    {
        using IEnumerator<TSource> e = source.GetEnumerator();
        while (e.MoveNext())
        {
            TSource[] chunk = new TSource[size];
            chunk[0] = e.Current;

            int i = 1;
            for (; i < chunk.Length && e.MoveNext(); i++)
            {
                chunk[i] = e.Current;
            }

            if (i == chunk.Length)
            {
                yield return chunk;
            }
            else
            {
                Array.Resize(ref chunk, i);
                yield return chunk;
                yield break;
            }
        }
    }
}