using System.Collections.Generic;

namespace DataCommander.Api;

public sealed class GetCompletionResult(int startPosition, int length, List<IObjectName> items, bool fromCache)
{
    public readonly int StartPosition = startPosition;
    public readonly int Length = length;
    public readonly List<IObjectName> Items = items;
    public readonly bool FromCache = fromCache;
}