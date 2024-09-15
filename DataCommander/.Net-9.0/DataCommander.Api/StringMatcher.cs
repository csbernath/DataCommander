using System;

namespace DataCommander.Api;

public sealed class StringMatcher(string subString, StringComparison comparison) : IStringMatcher
{
    public bool IsMatch(string text) => text.Contains(subString, comparison);
}