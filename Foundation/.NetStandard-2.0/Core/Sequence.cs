namespace Foundation
{
    public sealed class Sequence
    {
        private int _index;

        public int Next()
        {
            var next = _index;
            ++_index;
            return next;
        }

        public void Reset() => _index = 0;
    }
}