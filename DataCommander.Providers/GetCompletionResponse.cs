namespace DataCommander.Providers
{
    using System.Collections.Generic;

    public sealed class GetCompletionResponse
    {
        public int StartPosition;
        public int Length;
        public List<IObjectName> Items;
        public bool FromCache;
    }
}