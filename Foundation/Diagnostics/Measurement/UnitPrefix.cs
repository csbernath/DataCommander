using System;

namespace Foundation.Diagnostics.Measurement;

/// <summary>
/// https://en.wikipedia.org/wiki/Binary_prefix
/// </summary>
[CLSCompliant(false)]
public class UnitPrefix(string name, string symbol, ulong @base)
{
    public readonly string Name = name;    
    public readonly string Symbol = symbol;
    public readonly ulong Base = @base;    
}