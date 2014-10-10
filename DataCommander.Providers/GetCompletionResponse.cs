namespace DataCommander.Providers
{
    public sealed class GetCompletionResponse
    {
        public int StartPosition;
        public int Length;
        public IObjectName[] Items;
        public bool FromCache;
    }
}