using System;

namespace Foundation.Collections;

public sealed class CSharpType
{
    public readonly string Name;
    public readonly Type Type;

    public CSharpType(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}