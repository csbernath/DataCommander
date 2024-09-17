namespace Foundation.Core;

public sealed class Sequence
{
    private int _index;

    public int Next()
    {
        int next = _index;
        ++_index;
        return next;
    }

    public void Reset() => _index = 0;
}