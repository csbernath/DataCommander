using System;

namespace Foundation.Linq;

public static class IdentityFunction<TElement>
{
    public static Func<TElement, TElement> Instance => x => x;
}