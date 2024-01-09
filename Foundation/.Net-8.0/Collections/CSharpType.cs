using System;

namespace Foundation.Collections;

public sealed class CSharpType(string name, Type type)
{
    public readonly string Name = name;
    public readonly Type Type = type;
}