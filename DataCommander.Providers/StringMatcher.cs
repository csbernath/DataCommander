namespace DataCommander
{
    using System;

    internal interface IStringMatcher
    {
        bool IsMatch(string text);
    }

    internal sealed class StringMatcher : IStringMatcher
    {
        private readonly string subString;
        private readonly StringComparison comparison;

        public StringMatcher(string subString, StringComparison comparison)
        {
            this.subString = subString;
            this.comparison = comparison;
        }

        public bool IsMatch(string text)
        {
            int index = text.IndexOf(this.subString, this.comparison);
            return index >= 0;
        }
    }
}