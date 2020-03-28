using System.Collections.Generic;
using DataCommander.Providers2;

namespace DataCommander.Providers
{
    public sealed class GetCompletionResponse
    {
        public int StartPosition;
        public int Length;
        public List<IObjectName> Items;
        public bool FromCache;
    }
}