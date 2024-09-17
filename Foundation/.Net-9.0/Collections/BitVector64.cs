using System;

namespace Foundation.Collections;

[method: CLSCompliant(false)]
public struct BitVector64(ulong data)
{
    [CLSCompliant(false)]
    public ulong Value { get; private set; } = data;

    public bool this[int index]
    {
        readonly get
        {
            ulong bit = 1UL << index;
            return (Value & bit) == bit;
        }

        set
        {
            ulong bit = 1UL << index;

            if (value)
                Value |= bit;
            else
                Value &= ~bit;
        }
    }

    public override readonly string ToString()
    {
        string value = Value.ToString("X");
        value = value.PadLeft(16, '0');
        return value;
    }
}