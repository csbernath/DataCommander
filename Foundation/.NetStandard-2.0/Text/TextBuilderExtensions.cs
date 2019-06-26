using System;
using Foundation.Core;

namespace Foundation.Text
{
    public static class TextBuilderExtensions
    {
        public static IDisposable AddCSharpBlock(this TextBuilder indentedTextBuilder) => indentedTextBuilder.AddBlock("{", "}");

        private static IDisposable AddBlock(this TextBuilder indentedTextBuilder, string begin, string end)
        {
            indentedTextBuilder.Add(begin);
            var indentation = indentedTextBuilder.Indent(1);

            return new Disposer(() =>
            {
                indentation.Dispose();
                indentedTextBuilder.Add(end);
            });
        }
    }
}