using System;
using Foundation.Core;

namespace Foundation.Text;

public static class TextBuilderExtensions
{
    public static IDisposable AddCSharpBlock(this TextBuilder indentedTextBuilder) => indentedTextBuilder.AddBlock("{", "}");

    public static IDisposable AddBlock(this TextBuilder indentedTextBuilder, string begin, string end)
    {
        indentedTextBuilder.Add(begin);
        IDisposable indentation = indentedTextBuilder.Indent(1);

        return new Disposer(() =>
        {
            indentation.Dispose();
            indentedTextBuilder.Add(end);
        });
    }
}