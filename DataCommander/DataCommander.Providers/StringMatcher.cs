using System;

namespace DataCommander.Providers
{
    internal sealed class StringMatcher : IStringMatcher
    {
        private readonly string _subString;
        private readonly StringComparison _comparison;

        public StringMatcher(string subString, StringComparison comparison)
        {
            _subString = subString;
            _comparison = comparison;
        }

        public bool IsMatch(string text)
        {
            var index = text.IndexOf(_subString, _comparison);
            return index >= 0;
        }
    }
}