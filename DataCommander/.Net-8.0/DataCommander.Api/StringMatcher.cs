using System;

namespace DataCommander.Api;

public sealed class StringMatcher(string subString, StringComparison comparison) : IStringMatcher
{
    public bool IsMatch(string text)
    {
        var index = text.IndexOf(subString, comparison);
        return index >= 0;
    }
}