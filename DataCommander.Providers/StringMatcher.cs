namespace DataCommander.Providers
{
    using System;

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
            var index = text.IndexOf(this.subString, this.comparison);
            return index >= 0;
        }
    }
}