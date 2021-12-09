using System;

namespace Foundation.Collections;

public struct BitVector64
{
    [CLSCompliant(false)]
    public BitVector64(ulong data) => Value = data;

    [CLSCompliant(false)]
    public ulong Value { get; private set; }

    public bool this[int index]
    {
        get
        {
            var bit = 1UL << index;
            return (Value & bit) == bit;
        }

        set
        {
            var bit = 1UL << index;

            if (value)
                Value |= bit;
            else
                Value &= ~bit;
        }
    }

    public override string ToString()
    {
        var value = Value.ToString("X");
        value = value.PadLeft(16, '0');
        return value;
    }
}