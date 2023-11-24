using System.Collections.Generic;

namespace DataCommander.Api;

public sealed class GetCompletionResponse
{
    public int StartPosition;
    public int Length;
    public List<IObjectName> Items;
    public bool FromCache;
}